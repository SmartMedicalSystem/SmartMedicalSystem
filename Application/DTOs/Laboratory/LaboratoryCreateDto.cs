using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Laboratory
{
    public class LaboratoryCreateDto
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$",
        ErrorMessage = "Phone number must start with 010, 011, 012, or 015 followed by 8 digits.")]
        public string Phone { get; set; } = null!;
        public LabStatus Status { get; set; } =LabStatus.Active;
        public int? HeadTechnicianId { get; set; }
        public int? DepartmentId { get; set; }
        public string? Code { get; set; }          
        public string? Specialty { get; set; }
    }
}
