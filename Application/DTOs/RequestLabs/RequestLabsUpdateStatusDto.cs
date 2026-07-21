using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.DTOs.RequestLabs
{

    public class RequestLabsUpdateStatusDto
    {
        public LabRequestStatus Status { get; set; }
    }
}
