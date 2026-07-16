using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly ILogger<AuditService> _logger;

        public AuditService(ILogger<AuditService> logger)
        {
            _logger = logger;
        }

        public Task LogAuthenticationEvent(string? userId, string eventType, string details)
        {
            _logger.LogInformation("[Audit][Auth] User:{UserId} Event:{Event} Details:{Details}", userId ?? "anonymous", eventType, details);
            return Task.CompletedTask;
        }

        public Task LogAuthorizationEvent(string? userId, string permission, bool success, string details)
        {
            _logger.LogInformation("[Audit][AuthZ] User:{UserId} Permission:{Permission} Success:{Success} Details:{Details}", userId ?? "anonymous", permission, success, details);
            return Task.CompletedTask;
        }

        public Task LogRateLimitEvent(string key, string details)
        {
            _logger.LogWarning("[Audit][RateLimit] Key:{Key} Details:{Details}", key, details);
            return Task.CompletedTask;
        }
    }
}
