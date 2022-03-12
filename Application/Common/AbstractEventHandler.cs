using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReportSharp.Reporters;

namespace Application.Common;

public abstract class AbstractEventHandler<T> : INotificationHandler<T>
    where T : INotification
{
    private IMediator _mediator;

    private IEnumerable<IDataReporter> DataReporters;
    private HttpContext HttpContext;

    public AbstractEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        DbContext = ServiceProvider.GetService<AppDbContext>();
        Logger = ServiceProvider.GetService<ILogger<AbstractEventHandler<T>>>();
        DataReporters = ServiceProvider.GetServices<IDataReporter>();
        var httpContextAccessor = ServiceProvider.GetService<IHttpContextAccessor>();
        if (httpContextAccessor != null) {
            HttpContext = httpContextAccessor.HttpContext;
        }
    }

    protected IServiceProvider ServiceProvider { get; }
    protected AppDbContext DbContext { get; }
    protected ILogger<AbstractEventHandler<T>> Logger { get; }
    protected IMediator Mediator => _mediator ??= ServiceProvider.GetService<IMediator>();

    public async Task Handle(T @event, CancellationToken _)
    {
        try {
            await HandleEvent(@event, _);
        }
        catch (Exception e) {
            Logger.LogError(e, $"Error in {GetType()}:");
            DataReporters.ToList()
                .ForEach(x =>
                    x.ReportData(HttpContext, "Exception", e.ToString())
                );
        }
    }

    public abstract Task HandleEvent(T @event, CancellationToken _);
}