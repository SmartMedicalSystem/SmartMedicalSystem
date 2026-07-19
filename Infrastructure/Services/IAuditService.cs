using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAuditService
    {
        Task LogAuthenticationEvent(string? userId, string eventType, string details);
        Task LogAuthorizationEvent(string? userId, string permission, bool success, string details);
        Task LogRateLimitEvent(string key, string details);
    }
}
