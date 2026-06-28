using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Doctor
{
    public class UpdateDoctorDTO
    {
        public string Name { get; set; }

        public string Specialization { get; set; }

        public string Contact { get; set; }

        public int DepartmentId { get; set; }
    }
}
