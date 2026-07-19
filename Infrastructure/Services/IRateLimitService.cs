using System;

namespace Infrastructure.Services
{
    public interface IRateLimitService
    {
        /// <summary>
        /// Returns true when the key exceeded the allowed quota. If limited, retryAfter contains time until window resets.
        /// </summary>
        bool IsLimitExceeded(string key, out TimeSpan retryAfter);

        /// <summary>
        /// Record a request for the given key.
        /// </summary>
        void IncrementRequest(string key);
    }
}
