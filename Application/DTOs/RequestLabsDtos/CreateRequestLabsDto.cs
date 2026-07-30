using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestLabsDtos
{
    public class CreateRequestLabsDto
    {
        public int SessionId { get; set; }

        public int LabTestId { get; set; }
    }
}
