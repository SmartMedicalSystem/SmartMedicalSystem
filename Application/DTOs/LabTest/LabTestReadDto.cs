namespace Application.DTOs.LabTest
{
    public record LabTestReadDto(int Id)
    {
        public string TestName { get; set; } = null!;
        public string Description { get; set; } = null!;

        //public Domain.Enums.LabTestStatus Status { get; set; } = Domain.Enums.LabTestStatus.Pending;
    }
}
