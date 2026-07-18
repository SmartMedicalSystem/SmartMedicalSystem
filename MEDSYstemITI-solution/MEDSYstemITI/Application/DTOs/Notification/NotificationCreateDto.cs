using System;

namespace Application.DTOs.Notification
{
    public class NotificationCreateDto
    {
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
    }
}
