using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public  class RequestLabs :BaseEntity
    {
        public int SessionId { get; set; }

        public int LabTestId { get; set; }

        public LabRequestStatus Status { get; set; }

        public Session Session { get; set; } = null!;

        public LabTest LabTest { get; set; } = null!;

    }
}
