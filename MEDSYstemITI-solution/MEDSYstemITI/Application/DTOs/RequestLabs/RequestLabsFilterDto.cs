using Domain.Enums;

namespace Application.DTOs.RequestLabs
{
    /// <summary>Bound from query string: Status / Priority / Test / Doctor dropdowns + search box.</summary>
    public class RequestLabsFilterDto
    {
        public LabRequestStatus? Status { get; set; }
        public LabRequestPriority? Priority { get; set; }
        public int? LabTestId { get; set; }
        public int? DoctorId { get; set; }
        public string? SearchTerm { get; set; }
    }
}