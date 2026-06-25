using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.PatientResultDtos
{
    public class CreatePatientResultDto
    {
        public int PatientId { get; set; }

        public int SessionId { get; set; }

        public int TestId { get; set; }

        public string AIClassifiedReport { get; set; } = string.Empty;

        public string AISuggestion { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public List<PatientResultElementDto> Elements { get; set; }
            = new();
    }
}
