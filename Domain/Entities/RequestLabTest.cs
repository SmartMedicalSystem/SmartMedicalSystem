using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class RequestLabTest : BaseEntity
    {
        public int RequestLabId { get; set; }
        public RequestLabs RequestLab { get; set; } = null!;

        public int LabTestId { get; set; }
        public LabTest LabTest { get; set; } = null!;

        public RequestLabTestStatus Status { get; set; } = RequestLabTestStatus.Pending;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public RequestLabTest() { }

        public RequestLabTest(int requestLabId, int labTestId)
        {
            RequestLabId = requestLabId;
            LabTestId = labTestId;
            CreatedAt = DateTime.UtcNow;
            Status = RequestLabTestStatus.Pending;
        }

        public void UpdateStatus(RequestLabTestStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
