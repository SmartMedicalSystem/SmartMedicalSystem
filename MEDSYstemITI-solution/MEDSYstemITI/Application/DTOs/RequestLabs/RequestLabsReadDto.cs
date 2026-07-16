using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.DTOs.RequestLabs
{
    public class RequestLabsReadDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }

        // Patient columns (PATIENT / SSN)
        public int PatientId { get; set; }
        public string PatientName { get; set; } = null!;
        public string SSN { get; set; } = null!;

        // Requested test column
        public List<int> LabTestIds { get; set; } = new();
        public List<string> TestNames { get; set; } = new();

        // Requested by column (doctor + department)
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string? DepartmentName { get; set; }

        public LabRequestPriority Priority { get; set; }
        public DateTime RequestedAt { get; set; }
        public LabRequestStatus Status { get; set; }
    }
}