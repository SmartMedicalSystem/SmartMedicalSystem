using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.SessionDto
{
    public class UpdateSessionDto
    {
        public int SessionId { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public int DeptId { get; set; }

        public DateTime SessionDate { get; set; }

        public string? Notes { get; set; }
    }
}
