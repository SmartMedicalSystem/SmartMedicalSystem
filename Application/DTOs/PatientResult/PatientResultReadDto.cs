using Domain.Enums;
using System;

namespace Application.DTOs.PatientResult
{

    public class PatientResultReadDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        /// <summary>Full name of the patient (FirstName + LastName).</summary>
        public string PatientName { get; set; } = string.Empty;
        public int SessionId { get; set; }
        /// <summary>The date the session/lab test was performed.</summary>
        public DateTime SessionDate { get; set; }
        public int LabTestId { get; set; }
        public string Summary { get; set; } = null!;
        public string AIClassifiedReport { get; set; } = null!;
        public string AISuggestion { get; set; } = null!;
        public PatinetResultAIReportStatus AIReportStatus { get; set; }
    }
}
