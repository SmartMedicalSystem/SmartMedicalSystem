namespace Application.DTOs.Department
{
    public class DepartmentReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string DepartmentMangager { get; set; } = null!;
        public int? DoctorId { get; set; }
    }
}
