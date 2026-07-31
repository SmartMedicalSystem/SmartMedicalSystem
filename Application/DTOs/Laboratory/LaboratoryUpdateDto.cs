using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Laboratory
{
    public class LaboratoryUpdateDto
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public LabStatus? Status { get; set; }
        public int? HeadTechnicianId { get; set; }
        public int? DepartmentId { get; set; }
        public string? Code { get; set; }          
        public string? Specialty { get; set; }
    }
}
