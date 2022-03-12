using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly HttpContext _httpContext;
    private readonly ILogger<TRequest> _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        HttpContext httpContext
    )
    {
        _timer = new Stopwatch();
        _logger = logger;
        _httpContext = httpContext;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500) {
            var requestName = typeof(TRequest).Name;
            var userId = _httpContext.Items["user"];
            var userName = string.Empty;

            _logger.LogWarning(
                "clean_test Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId ?? "anonymous", userName, request);
        }

        return response;
    }
}