using Domain.Enums;

namespace Application.DTOs.RequestLabs
{
    public class RequestLabsReadDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public DateTime RequestedAt { get; set; }
        public LabRequestStatus Status { get; set; }
        public List<int> LabTestIds { get; set; } = new();
    }
}
