using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Doctor
{
    public class DoctorForSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
