using Application.Common.Response;
using Infrastructure;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common;

public abstract class AbstractRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private IMediator _mediator;

    protected AbstractRequestHandler(IAuthorizationService authorizationService, AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        AuthorizationService = authorizationService;
        DbContext = dbContext;
        HttpContext = httpContextAccessor?.HttpContext;
    }

    public IAuthorizationService AuthorizationService { get; set; }

    protected AppDbContext DbContext { get; }

    protected HttpContext HttpContext { get; }

    public object AuthenticatedUser => AuthorizationService?.GetUser();

    public IMediator Mediator {
        get => _mediator ??= HttpContext?.RequestServices.GetService<IMediator>();
        set => _mediator = value;
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken _);

    protected StdResponse<T> Ok<T>(T data = default, string msg = null)
    {
        return StdResponseFactory.Ok<T>(data, msg);
    }

    protected StdResponse<T> OkMsg<T>(string msg = null)
    {
        return StdResponseFactory.OkMsg<T>(msg);
    }

    protected StdResponse<T> NotFound<T>(object data = default, string msg = "Notfound")
    {
        return StdResponseFactory.NotFound<T>(data, msg);
    }

    protected StdResponse<T> NotFoundMsg<T>(string msg = "Notfound")
    {
        return StdResponseFactory.NotFoundMsg<T>(msg);
    }

    protected StdResponse<T> PermissionDenied<T>(object data = default, string msg = null)
    {
        return StdResponseFactory.PermissionDenied<T>(data, msg);
    }

    protected StdResponse<T> PermissionDeniedMsg<T>(string msg = null)
    {
        return StdResponseFactory.PermissionDeniedMsg<T>(msg);
    }

    protected StdResponse<T> NotAuth<T>(object data = default, string msg = "Not authorized.")
    {
        return StdResponseFactory.NotAuth<T>(data, msg);
    }

    protected StdResponse<T> NotAuthMsg<T>(string msg = "Not authorized.")
    {
        return StdResponseFactory.NotAuthMsg<T>(msg);
    }

    protected StdResponse<T> BadRequest<T>(object data = default, string msg = null)
    {
        return StdResponseFactory.BadRequest<T>(data, msg);
    }

    protected StdResponse<T> BadRequestMsg<T>(string msg = null)
    {
        return StdResponseFactory.BadRequestMsg<T>(msg);
    }

    protected StdResponse<T> SyntaxError<T>(object data = default, string msg = null)
    {
        return StdResponseFactory.SyntaxError<T>(data, msg);
    }

    protected StdResponse<T> SyntaxErrorMsg<T>(string msg = null)
    {
        return StdResponseFactory.SyntaxErrorMsg<T>(msg);
    }

    protected StdResponse<T> InternalError<T>(object data = default, string msg = null)
    {
        return StdResponseFactory.InternalError<T>(data, msg);
    }

    protected StdResponse<T> InternalErrorMsg<T>(string msg = null)
    {
        return StdResponseFactory.InternalErrorMsg<T>(msg);
    }
}