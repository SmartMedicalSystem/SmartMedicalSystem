using Domain.Common;
using Domain.Identity;
using System;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get;  set; }

        public string Message { get;  set; } = null!;

        public DateTime SentAt { get;  set; }

        public bool IsRead { get;  set; }

        public ApplicationUser User { get; set; } = null!;

        private Notification() { }

        public Notification(int userId, string message, DateTime sentAt)
        {
            UserId = Guard.Positive(userId, nameof(userId));
            Message = Guard.NotNullOrWhiteSpace(message, nameof(message), 1000);
            SentAt = Guard.NotDefault(sentAt, nameof(sentAt));
            IsRead = false;
        }

        public void MarkAsRead() => IsRead = true;
    }
}
