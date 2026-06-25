using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Domain.Entities
{
    public class Session
    {
        public int SessionId { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public int DeptId { get; set; }

        public DateTime SessionDate { get; set; }

        public string? Notes { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; } = null!;

        public Doctor Doctor { get; set; } = null!;

        public Department Department { get; set; } = null!;
    }
}
