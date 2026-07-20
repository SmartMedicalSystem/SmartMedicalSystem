namespace Application.DTOs.RequestLabs
{
    public class RequestLabsStatisticsDto
    {
        public int TotalRequests { get; set; }

        public int PendingRequests { get; set; }

        public int CompletedToday { get; set; }
    }
}
