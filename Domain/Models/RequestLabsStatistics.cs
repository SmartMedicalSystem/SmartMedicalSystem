namespace Domain.Models
{
    public class RequestLabsStatistics
    {
        public int TotalRequests { get; set; }

        public int PendingRequests { get; set; }

        public int CompletedToday { get; set; }
    }
}
