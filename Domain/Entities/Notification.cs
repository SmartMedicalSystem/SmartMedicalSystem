using Domain.Common;
using Domain.Enums;
using Domain.Identity;
using System;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }

        public string Message { get; set; } = null!;

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }
        public NotificationType Type { get; private set; }

        public int? RequestLabsId { get; private set; }

        public RequestLabs? RequestLabs { get; private set; }

        public int? PatientResultId { get; private set; }

        public PatientResult? PatientResult { get; private set; }

        public ApplicationUser User { get; set; } = null!;

        private Notification() { }

        public Notification(
            int userId,
            string message,
            NotificationType type,
            int? requestLabsId = null,
            int? patientResultId = null)
        {
            UserId = Guard.Positive(userId, nameof(userId));

            Message = Guard.NotNullOrWhiteSpace(
                message,
                nameof(message),
                1000);

            Type = type;

            RequestLabsId = requestLabsId;

            PatientResultId = patientResultId;

            SentAt = DateTime.UtcNow;

            IsRead = false;
        }

        public void MarkAsRead() => IsRead = true;
    }
}