using Microsoft.AspNetCore.Authorization;

namespace Infrastructure;

public class Policies
{
    public const string User = "User";
    public const string Admin = "Admin";
    public const string UserSwagger = "V1 User";
    public const string AdminSwagger = "V1 Admin";

    public static AuthorizationPolicy UserPolicy()
    {
        return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(User).Build();
    }

    public static AuthorizationPolicy AdminPolicy()
    {
        return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();
    }
}