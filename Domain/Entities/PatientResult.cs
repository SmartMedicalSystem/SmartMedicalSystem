using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class PatientResult 
    {
        public int ResultId { get; set; }

        public int PatientId { get; set; }

        public int SessionId { get; set; }

        public int TestId { get; set; }

        public string AIClassifiedReport { get; set; } = string.Empty;

        public string AISuggestion { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public DateTime ResultDate { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; } = null!;

        public Session Session { get; set; } = null!;

        public MedicalTest Test { get; set; } = null!;

        public ICollection<PatientResultElement> ResultElements { get; set; }
            = new List<PatientResultElement>();
    }
}
