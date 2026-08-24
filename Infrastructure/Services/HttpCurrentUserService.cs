using Application.Services.Abstraction;
using Domain.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services;

public sealed class HttpCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var value = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? user?.FindFirstValue("sub")
                        ?? user?.FindFirstValue("nameid");
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public int? BasePersonId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(CustomClaimTypes.BasePersonId)
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("base_person_id");
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public bool HasPermission(string permission)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.Claims.Any(c => c.Type == CustomClaimTypes.Permission && c.Value == permission) == true;
    }
}
