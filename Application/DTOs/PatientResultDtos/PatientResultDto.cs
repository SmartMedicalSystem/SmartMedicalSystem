using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.PatientResultDtos
{
    public class PatientResultDto
    {
        public int ResultId { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public string TestName { get; set; } = string.Empty;

        public DateTime ResultDate { get; set; }

        public string AIClassifiedReport { get; set; } = string.Empty;

        public string AISuggestion { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public List<PatientResultElementDetailsDto> Elements { get; set; }
            = new();
    }
}
