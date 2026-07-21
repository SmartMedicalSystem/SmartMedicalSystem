using Domain.Enums;

namespace Application.DTOs.RequestLabs
{
    public class RequestLabsReadDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public DateTime RequestedAt { get; set; }
        public LabRequestStatus Status { get; set; }
        public LabRequestPriority Priority { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Patient information
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientSSN { get; set; } = string.Empty;
        public int PatientAge { get; set; }

        // Doctor information
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string? DoctorDepartment { get; set; }


        // Requested lab tests details
        public List<Application.DTOs.LabTest.LabTestReadDto> LabTests { get; set; } = new();
        public List<int> LabTestIds { get; set; }
    }
}
