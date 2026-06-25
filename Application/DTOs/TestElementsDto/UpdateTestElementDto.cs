using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.TestElementsDto
{
    public class UpdateTestElementDto
    {
        public string ElementName { get; set; }
        public string Unit { get; set; }
        public float NormalMin { get; set; }
        public float NormalMax { get; set; }
    }
}
