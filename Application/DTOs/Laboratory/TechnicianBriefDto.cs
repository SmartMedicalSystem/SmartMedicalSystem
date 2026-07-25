using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Laboratory
{
    public class TechnicianBriefDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? LaboratoryName { get; set; }

    }
}
