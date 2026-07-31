using Application.DTOs.LabTechnician;
using Application.DTOs.LabTest;
using Domain.Enums;

namespace Application.DTOs.Laboratory
{
    public class LaboratoryReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Code { get; set; }
        public string? Specialty { get; set; }
        public LabStatus? Status { get; set; }
        public int? HeadTechnicianId { get; set; }
        public string? HeadTechnicianName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int TestCount { get; set; }
        public int TechnicianCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<TestDto> LabTests { get; set; } = new();
        public List<LabTechnicianReadDto> Technicians { get; set; } = new();
    }
}