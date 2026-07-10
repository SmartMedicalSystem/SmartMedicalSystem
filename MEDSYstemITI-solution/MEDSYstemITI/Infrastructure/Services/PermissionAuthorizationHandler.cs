using Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Services
{
    /// <summary>
    /// Satisfies a PermissionRequirement when the authenticated user's token
    /// carries a "Permission" claim equal to the required permission's name.
    /// Permission claims are added to the JWT at login time (see TokenService /
    /// AuthService), one per permission granted to the user's role, so this
    /// handler only needs to inspect the current ClaimsPrincipal - no DB call.
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var hasPermission = context.User.Claims.Any(c =>
                c.Type == "Permission" &&
                string.Equals(c.Value, requirement.Permission.ToString(), StringComparison.OrdinalIgnoreCase));

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
