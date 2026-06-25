using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.SessionDto
{
    public class SessionDto
    {
        public int SessionId { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public DateTime SessionDate { get; set; }

        public string? Notes { get; set; }
    }
}
