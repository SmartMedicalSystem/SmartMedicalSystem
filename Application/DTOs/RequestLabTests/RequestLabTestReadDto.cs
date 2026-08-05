using System;

namespace Application.DTOs.RequestLabTests
{
    public class RequestLabTestReadDto
    {
        public int LabTestId { get; set; }
        public string LabTestName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
