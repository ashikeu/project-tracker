using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Infrastructure.Authentication;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Extensions;

public static class AuthorizationServiceExtensions
{
    /// <summary>Registers one authorization policy per permission key in PermissionKeys.All —
    /// controllers use [Authorize(Policy = PermissionKeys.TaskAssign)] etc. Policies are
    /// derived from the single source of truth list rather than hand-declared, so a new
    /// permission key automatically gets an enforceable policy.</summary>
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder();

        services.PostConfigure<AuthorizationOptions>(options =>
        {
            foreach (var permissionKey in PermissionKeys.All)
            {
                options.AddPolicy(permissionKey, policy =>
                    policy.RequireClaim(CustomClaimTypes.Permission, permissionKey));
            }
        });

        return services;
    }
}
