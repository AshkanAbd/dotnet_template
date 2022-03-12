using System.Security.Claims;
using Application.Common.Response;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using IAuthorizationService = Infrastructure.Authorization.IAuthorizationService;

namespace Presentation.Filters;

public class UserAuthorizeFilter : ActionFilterAttribute
{
    private readonly IAuthorizationService _authorizationService;
    private readonly AppDbContext _dbContext;

    public UserAuthorizeFilter(AppDbContext dbContext, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _authorizationService = authorizationService;
    }

    private HttpContext HttpContext { get; set; }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        HttpContext = context.HttpContext;
        var authorizedUser = context.HttpContext.User;
        if (!authorizedUser.Identity.IsAuthenticated) {
            await next();
            return;
        }

        var user = await GetAuthenticatedUser(authorizedUser);
        if (user == null) {
            context.Result = ResponseFormat.NotAuthMsg<object>();
            return;
        }

        var routePolicy = GetRoutePolicy(context.ActionDescriptor.EndpointMetadata);

        if (CheckRoutePolicy(authorizedUser, routePolicy)) {
            switch (CheckUserAccount(authorizedUser, user)) {
                case 0:
                    context.Result = ResponseFormat
                        .PermissionDeniedMsg<object>("Your account has been deactivated.");
                    return;
            }

            if (!await TokenExists(context.HttpContext.Request.Headers["Authorization"].ToString())) {
                context.Result = ResponseFormat.NotAuthMsg<object>();
                return;
            }

            _authorizationService.SetUser(user);
            await next();
        }
        else {
            context.Result = null;
            _authorizationService.SetUser(user);
            await next();
        }
    }

    private async Task<object> GetAuthenticatedUser(ClaimsPrincipal authorizedUser)
    {
        /*
        if (authorizedUser.IsInRole(Policies.Admin)) {
            return await _dbContext.Admins
                .Include(x => x.Role)
                .ThenInclude(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(x =>
                    x.Id == long.Parse(authorizedUser.FindFirstValue("id")));
        }
        */
        if (authorizedUser.IsInRole(Policies.User)) {
            /*
            return await _dbContext.Users.FirstOrDefaultAsync(x =>
                x.Id == long.Parse(authorizedUser.FindFirstValue("id"))
            );
        */
        }

        return await Task.FromResult((object) null);
    }

    private int CheckUserAccount(ClaimsPrincipal authorizedUser, object authenticatedUser)
    {
        if (authorizedUser.IsInRole(Policies.Admin)) {
            return 1;
        }


        if (authorizedUser.IsInRole(Policies.User)) {
            /*
            var user = (User) authenticatedUser;
            if (!user.Active) {
                return 0;
            }

            if (user.UserType == null &&
                HttpContext.Request.Path.Value != "/api/v1/client/auth/complete" &&
                HttpContext.Request.Path.Value != "/api/v1/client/auth/complete/"
            ) {
                return 2;
            }
            */

            return 1;
        }

        return 0;
    }

    private async Task<bool> TokenExists(string accessToken)
    {
        // return await _dbContext.Tokens.AnyAsync(token => token.AccessToken == accessToken);
        return await Task.FromResult(true);
    }

    private bool CheckRoutePolicy(ClaimsPrincipal authorizedUser, string routePolicy)
    {
        return routePolicy != null && authorizedUser.IsInRole(routePolicy);
    }

    private string GetRoutePolicy(IEnumerable<object> endPointMetadata)
    {
        var authorizeAttributes = endPointMetadata.OfType<AuthorizeAttribute>().ToList();
        if (authorizeAttributes.Count == 0) return null;
        var authorizeAttribute = authorizeAttributes[0];
        return authorizeAttribute.Policy;
    }
}