namespace Infrastructure.Services;

public interface IRateLimitService
{
    bool IsAllowed(string key, int maxRequests, TimeSpan window);
}
