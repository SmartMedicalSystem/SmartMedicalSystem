using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.PatientResultElementDtos
{
    public class PatientResultElementDetailsDto
    {
        public string ElementName { get; set; } = string.Empty;

        public double Value { get; set; }

        public string TechnicianName { get; set; } = string.Empty;
    }
}
