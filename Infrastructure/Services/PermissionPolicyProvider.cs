using Application.Services.Auth;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    /// <summary>
    /// Recognizes policy names of the form "Permission:{PermissionName}" (produced by
    /// CheckPermissionAttributeAbstract / HasPermissionAttribute) and builds the matching
    /// AuthorizationPolicy with a PermissionRequirement on the fly, instead of requiring
    /// every permission to be pre-registered with services.AddAuthorization(...).
    /// </summary>
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(CheckPermissionAttributeAbstract.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var permissionName = policyName.Substring(CheckPermissionAttributeAbstract.PolicyPrefix.Length);

                if (Enum.TryParse<Permissions>(permissionName, out var permission))
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddRequirements(new PermissionRequirement(permission))
                        .Build();

                    return Task.FromResult<AuthorizationPolicy?>(policy);
                }
            }

            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
