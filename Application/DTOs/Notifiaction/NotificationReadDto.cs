using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Notifiaction
{
    public class NotificationReadDto
    {
        public int Id { get; set; }

        public string Message { get; set; } = null!;

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }
    }
}
