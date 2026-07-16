using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.DTOs.RequestLabs
{
    public class RequestLabsCreateDto
    {
        public int SessionId { get; set; }
        public DateTime RequestedAt { get; set; }
        public LabRequestPriority Priority { get; set; } = LabRequestPriority.Routine;
        public List<int> LabTestIds { get; set; } = new();
    }
}