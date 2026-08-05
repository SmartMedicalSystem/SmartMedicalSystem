namespace Application.DTOs.LabTest
{
    public class LabTestUpdateDto
    {
        public string TestName { get; set; } = null!;
        public string Description { get; set; } = null!;

        public Domain.Enums.LabTestStatus Status { get; set; } = Domain.Enums.LabTestStatus.Pending;
    }
}
