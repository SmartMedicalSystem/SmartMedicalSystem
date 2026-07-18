using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class RequestLabs : BaseEntity
    {
        public int SessionId { get;  set; }

        public DateTime RequestedAt { get;  set; }

        public LabRequestStatus Status { get;  set; }

        public virtual Session Session { get;  set; } = null!;

        public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();

        private RequestLabs() { }

        public RequestLabs(int sessionId, DateTime requestedAt)
        {
            SessionId = Guard.Positive(sessionId, nameof(sessionId));
            RequestedAt = Guard.NotDefault(requestedAt, nameof(requestedAt));
            Status = LabRequestStatus.Pending;
        }

        public void UpdateStatus(LabRequestStatus status)
        {
            Status = status;
        }
    }
}
