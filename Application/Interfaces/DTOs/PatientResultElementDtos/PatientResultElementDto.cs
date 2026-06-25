using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.PatientResultElementDtos
{
    public class PatientResultElementDto
    {
        public int TestElementId { get; set; }

        public double Value { get; set; }

        public int TechId { get; set; }
    }
}
