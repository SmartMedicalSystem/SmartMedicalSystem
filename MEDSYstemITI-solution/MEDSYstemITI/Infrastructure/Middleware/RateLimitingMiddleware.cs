using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Infrastructure.Middleware
{
    /// <summary>
    /// Middleware that applies a simple per-key rate limit and records audit events when limits are exceeded.
    /// Register the middleware in Program.cs: app.UseMiddleware<RateLimitingMiddleware>();
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimitService _rateLimit;
        private readonly IAuditService _audit;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        public RateLimitingMiddleware(RequestDelegate next, IRateLimitService rateLimit, IAuditService audit, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _rateLimit = rateLimit;
            _audit = audit;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = context.User?.FindFirst("sub")?.Value ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

            if (_rateLimit.IsLimitExceeded(key, out var retryAfter))
            {
                _logger.LogWarning("Rate limit exceeded for {Key}", key);
                await _audit.LogRateLimitEvent(key, $"Rate limit exceeded. Retry after {retryAfter.TotalSeconds} seconds.");

                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers["Retry-After"] = ((int)retryAfter.TotalSeconds).ToString();
                await context.Response.WriteAsync("Too many requests. Please retry later.");
                return;
            }

            _rateLimit.IncrementRequest(key);
            await _next(context);
        }
    }
}
