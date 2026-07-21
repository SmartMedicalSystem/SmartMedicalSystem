namespace Application.DTOs.RequestLabs
{
    public class RequestLabsCreateDto
    {
        public int SessionId { get; set; }
        public DateTime RequestedAt { get; set; }
        public List<int> LabTestIds { get; set; } = new();
        public Domain.Enums.LabRequestPriority Priority { get; set; } = Domain.Enums.LabRequestPriority.Normal;
    }
}
