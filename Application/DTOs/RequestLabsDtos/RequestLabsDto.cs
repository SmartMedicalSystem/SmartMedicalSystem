using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestLabsDtos
{
    public class RequestLabsDto
    {
        public int Id { get; set; }

        public int SessionId { get; set; }

        public int LabTestId { get; set; }

        public DateTime RequestedAt { get; set; }

        public string Status { get; set; }

    }
}
