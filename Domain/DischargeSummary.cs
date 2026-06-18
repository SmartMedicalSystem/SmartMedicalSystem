public class DischargeSummary
{
    public int DischargeSummaryId { get; set; }

    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int DepartmentId { get; set; }

    public string SummaryText { get; set; } = string.Empty;
    public string PrescribedDrugs { get; set; } = string.Empty;

    public DateTime VisitDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public Department Department { get; set; } = null!;

    public ICollection<DischargeSummaryDisease> DischargeSummaryDiseases { get; set; }
        = new List<DischargeSummaryDisease>();
}