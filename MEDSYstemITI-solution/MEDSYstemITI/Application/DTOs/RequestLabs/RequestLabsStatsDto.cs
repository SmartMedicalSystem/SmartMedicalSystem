namespace Application.DTOs.RequestLabs
{
    public class RequestLabsStatsDto
    {
        public int TotalRequests { get; set; }
        public int PendingCount { get; set; }
        public int CompletedTodayCount { get; set; }
    }
}