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

        public LabRequestPriority Priority { get; set; }

        public DateTime? CompletedAt { get; set; }

        public virtual Session Session { get;  set; } = null!;

        public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();

        private RequestLabs() { }

        public RequestLabs(int sessionId, DateTime requestedAt, LabRequestPriority priority = LabRequestPriority.Normal)
        {
            SessionId = Guard.Positive(sessionId, nameof(sessionId));
            RequestedAt = Guard.NotDefault(requestedAt, nameof(requestedAt));
            Status = LabRequestStatus.Pending;
            Priority = priority;
        }

        public void UpdateStatus(LabRequestStatus status)
        {
            Status = status;
            if (status == LabRequestStatus.Completed)
                CompletedAt = DateTime.UtcNow;
            else
                CompletedAt = null;
        }
    }
}
