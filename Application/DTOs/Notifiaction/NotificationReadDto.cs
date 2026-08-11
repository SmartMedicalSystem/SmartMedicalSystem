using Domain.Enums;
using System;

namespace Application.DTOs.Notifiaction
{
    public class NotificationReadDto
    {
        public int Id { get; set; }

        public string Message { get; set; } = null!;

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }

        public NotificationType Type { get; set; }

        public int? RequestLabsId { get; set; }

        public int? PatientResultId { get; set; }

        public int? SessionId { get; set; }

        public int? PatientId { get; set; }
    }
}