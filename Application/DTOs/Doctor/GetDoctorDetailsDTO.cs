using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Doctor
{
    public class GetDoctorDetailsDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Specialization { get; set; }

        public string Contact { get; set; }

        public string DepartmentName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
