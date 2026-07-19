using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

namespace Infrastructure.Services
{
    /// <summary>
    /// Simple in-memory fixed window rate limiter. Not distributed — suitable for single-instance demos.
    /// </summary>
    public class InMemoryRateLimitService : IRateLimitService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<InMemoryRateLimitService> _logger;

        // Configure limits here or make configurable via options
        private readonly int _limit = 60; // requests
        private readonly TimeSpan _window = TimeSpan.FromMinutes(1);

        public InMemoryRateLimitService(IMemoryCache cache, ILogger<InMemoryRateLimitService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public bool IsLimitExceeded(string key, out TimeSpan retryAfter)
        {
            var entry = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = _window;
                return new RateLimitCounter { Count = 0, ExpiresAt = DateTimeOffset.UtcNow.Add(_window) };
            });

            retryAfter = entry.ExpiresAt - DateTimeOffset.UtcNow;
            return entry.Count >= _limit;
        }

        public void IncrementRequest(string key)
        {
            var entry = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = _window;
                return new RateLimitCounter { Count = 0, ExpiresAt = DateTimeOffset.UtcNow.Add(_window) };
            });

            entry.Count++;
            _cache.Set(key, entry, entry.ExpiresAt.UtcDateTime - DateTime.UtcNow);
        }

        private class RateLimitCounter
        {
            public int Count { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
        }
    }
}
