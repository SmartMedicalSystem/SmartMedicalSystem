using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Doctor
{
    public class DoctorAtDepartmentDto
    {
        public string Name { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public int Id { get; set; }
        //public int DepartmentId { get; set; }

    }
}
