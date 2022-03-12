using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters;

public class AdminPermission : ActionFilterAttribute
{
    private readonly string _permission;

    public AdminPermission(string permission)
    {
        _permission = permission;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        /*
        var authorize = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

        if (authorize == null) {
            await next();
            return;
        }

        var admin = (Admin) authorize.GetUser();

        if (!admin.Role.RolePermissions
            .Any(x => x.Permission.Code.Equals(_permission, StringComparison.CurrentCultureIgnoreCase))) {
            context.Result = ResponseFormat.PermissionDeniedMsg<object>("شما به این قسمت دسترسی ندارید.");
            return;
        }
        */

        await next();
    }
}