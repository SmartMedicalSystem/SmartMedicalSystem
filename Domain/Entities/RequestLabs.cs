using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class RequestLabs : BaseEntity
    {
        private RequestLabs() { }

        public RequestLabs(int sessionId, int labTestId)
        {
            if (sessionId <= 0)
                throw new ArgumentException(
                    "Session Id is required.",
                    nameof(sessionId));

            if (labTestId <= 0)
                throw new ArgumentException(
                    "Lab Test Id is required.",
                    nameof(labTestId));

            SessionId = sessionId;
            LabTestId = labTestId;

            RequestedAt = DateTime.UtcNow;
            Status = LabRequestStatus.Pending;
        }

        public int SessionId { get; private set; }

        public int LabTestId { get; private set; }

        public DateTime RequestedAt { get; private set; }

        public LabRequestStatus Status { get; private set; }

        public virtual Session Session { get; private set; }

        public virtual LabTest LabTest { get; private set; }

        public void StartProcessing()
        {
            if (Status != LabRequestStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending requests can be started.");
            }

            Status = LabRequestStatus.InProgress;
        }

        public void Complete()
        {
            if (Status != LabRequestStatus.InProgress)
            {
                throw new InvalidOperationException(
                    "Only in-progress requests can be completed.");
            }

            Status = LabRequestStatus.Completed;
        }

        public void Cancel()
        {
            if (Status == LabRequestStatus.Completed)
            {
                throw new InvalidOperationException(
                    "Completed requests cannot be cancelled.");
            }

            Status = LabRequestStatus.Cancelled;
        }
    }
}