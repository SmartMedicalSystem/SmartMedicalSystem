namespace Domain.Models
{
    /// <summary>
    /// Backing model for the 3 stat cards on top of the dashboard
    /// (Total Requests / Pending / Completed Today).
    /// </summary>
    public class RequestLabsStats
    {
        public int TotalRequests { get; }
        public int PendingCount { get; }
        public int CompletedTodayCount { get; }

        public RequestLabsStats(int totalRequests, int pendingCount, int completedTodayCount)
        {
            TotalRequests = totalRequests;
            PendingCount = pendingCount;
            CompletedTodayCount = completedTodayCount;
        }
    }
}
