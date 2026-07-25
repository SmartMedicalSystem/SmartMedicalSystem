using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestLabsDtos
{
    public class UpdateRequestLabsStatusDto
    {
        public LabRequestStatus Status { get; set; }
    }
}
