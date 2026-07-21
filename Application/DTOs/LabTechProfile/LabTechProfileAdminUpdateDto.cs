namespace Application.DTOs.LabTechProfile
{
    /// <summary>Admin-only update: Professional Information section (grayed-out fields on the tech's own page).</summary>
    public class LabTechProfileAdminUpdateDto
    {
        public string AssignedLaboratory { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public int YearsOfExperience { get; set; }
    }
}
