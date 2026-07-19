using System;

namespace Application.DTOs.Session
{

    public class SessionReadDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int DeptId { get; set; }
        public DateTime SessionDate { get; set; }
        public string? Notes { get; set; }
    }
}
