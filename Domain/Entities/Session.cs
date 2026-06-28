using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Session : BaseEntity
    {
        public Patient Patient { get; set; }
        public int PatientId { get; set; }
    }
}
