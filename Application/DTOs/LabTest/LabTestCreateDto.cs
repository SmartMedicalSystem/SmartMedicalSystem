using Domain.Enums;
namespace Application.DTOs.LabTest
{
    public class LabTestCreateDto
    {
        public string TestName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public LabTestStatus Status { get; set; } = LabTestStatus.Pending;
    }
}
