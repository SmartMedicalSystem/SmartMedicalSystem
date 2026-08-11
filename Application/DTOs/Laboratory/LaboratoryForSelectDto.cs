using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Laboratory
{
    public class LaboratoryForSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public LabStatus ?Status { get; set; } 
    }
}
