namespace Application.DTOs.LabTest
{
    public record LabTestReadDto(int Id)
    {
        public string TestName { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
