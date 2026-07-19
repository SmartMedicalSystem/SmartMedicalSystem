using Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Application.Services.Auth
{
    /// <summary>
    /// Requirement satisfied when the current user's JWT carries a "Permission"
    /// claim matching the required Permissions value.
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public Permissions Permission { get; }

        public PermissionRequirement(Permissions permission)
        {
            Permission = permission;
        }
    }
}
