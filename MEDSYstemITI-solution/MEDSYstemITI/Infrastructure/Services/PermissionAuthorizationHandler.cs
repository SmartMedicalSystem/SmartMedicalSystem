using Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

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
        private readonly IAuditService _audit;
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(IAuditService audit, ILogger<PermissionAuthorizationHandler> logger)
        {
            _audit = audit;
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var hasPermission = context.User.Claims.Any(c =>
                c.Type == "Permission" &&
                string.Equals(c.Value, requirement.Permission.ToString(), StringComparison.OrdinalIgnoreCase));

            var userId = context.User?.FindFirst("sub")?.Value ?? context.User?.Identity?.Name;

            try
            {
                _audit?.LogAuthorizationEvent(userId, requirement.Permission.ToString(), hasPermission, "Permission check");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to record authorization audit");
            }

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
