using Domain.Enums;

namespace Domain.Models
{
    /// <summary>
    /// Filter criteria for the Lab Requests dashboard grid
    /// (Status / Priority / Test / Doctor dropdowns + search box).
    /// </summary>
    public class RequestLabsFilterParams
    {
        public LabRequestStatus? Status { get; set; }
        public LabRequestPriority? Priority { get; set; }
        public int? LabTestId { get; set; }
        public int? DoctorId { get; set; }

        /// <summary>Matches patient name, SSN (NationalId) or doctor name.</summary>
        public string? SearchTerm { get; set; }
    }
}
