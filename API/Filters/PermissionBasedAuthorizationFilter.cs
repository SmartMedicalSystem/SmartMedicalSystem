using API.Attributes;
using Infrastructure.Context;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Filters
{
    public class PermissionBasedAuthorizationFilter(HttpContext httpContext , PermessionHandler permessionHandler) : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var attribute = context.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x is CheckPermissionAttribute);
            if (attribute != null)
            {
                ClaimsIdentity claims = httpContext.User.Identity as ClaimsIdentity;
                if (claims == null || !claims.IsAuthenticated)
                {
                    context.Result = new ForbidResult();
                }
                var result = permessionHandler.CheckPermission();

            }
        }
    }
}

