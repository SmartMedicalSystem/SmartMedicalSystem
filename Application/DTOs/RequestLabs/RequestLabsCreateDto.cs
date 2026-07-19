namespace Application.DTOs.RequestLabs
{
    public class RequestLabsCreateDto
    {
        public int SessionId { get; set; }
        public DateTime RequestedAt { get; set; }
        public List<int> LabTestIds { get; set; } = new();
    }
}
