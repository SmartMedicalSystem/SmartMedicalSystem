using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class PatientResult : BaseEntity
    {
        // FKs from Patient, Session and LabTest
        public int PatientId { get;  set; }

        public int SessionId { get;  set; }

        public int LabTestId { get;  set; }

        public string AIClassifiedReport { get;  set; } = string.Empty;

        public string AISuggestion { get;  set; } = string.Empty;

        public string Summary { get;  set; } = string.Empty;

        // Navigation Properties
        public Patient Patient { get; set; } = null!;

        public Session Session { get; set; } = null!;

        public LabTest labTest { get; set; } = null!; // naming kept exactly as in the source entity

        public ICollection<PatientResultElement> ResultElements { get; set; } = new List<PatientResultElement>();

        private PatientResult() { }

        public PatientResult(int patientId, int sessionId, int labTestId, string summary,
            string aiClassifiedReport = "", string aiSuggestion = "")
        {
            PatientId = Guard.Positive(patientId, nameof(patientId));
            SessionId = Guard.Positive(sessionId, nameof(sessionId));
            LabTestId = Guard.Positive(labTestId, nameof(labTestId));
            Summary = Guard.NotNullOrWhiteSpace(summary, nameof(summary), 2000);
            AIClassifiedReport = aiClassifiedReport ?? string.Empty;
            AISuggestion = aiSuggestion ?? string.Empty;
        }

        public void UpdateAIOutput(string aiClassifiedReport, string aiSuggestion, string summary)
        {
            AIClassifiedReport = aiClassifiedReport ?? string.Empty;
            AISuggestion = aiSuggestion ?? string.Empty;
            Summary = Guard.NotNullOrWhiteSpace(summary, nameof(summary), 2000);
        }
    }
}
