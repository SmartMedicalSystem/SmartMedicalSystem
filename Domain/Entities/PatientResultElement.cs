using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class PatientResultElement
    {
        public int ElementId { get; set; }

        public int ResultId { get; set; }

        public int TestElementId { get; set; }

        public double Value { get; set; }

        public int TechId { get; set; }

        // Navigation Properties
        public PatientResult Result { get; set; } = null!;

        public TestElement TestElement { get; set; } = null!;

        public LabTechnician Technician { get; set; } = null!;
    }
}
