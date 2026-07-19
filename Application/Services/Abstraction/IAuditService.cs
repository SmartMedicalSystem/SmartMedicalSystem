using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Abstraction
{  
        public interface IAuditService
        {
            Task LogAuthenticationEvent(string? userId, string eventType, string details);

            Task LogAuthorizationEvent(string? userId, string permission, bool success, string details);

            Task LogRateLimitEvent(string key, string details);
        }
    }


