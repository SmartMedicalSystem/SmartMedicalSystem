using Application.DTOs.Notifiaction;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Abstraction
{
    public interface INotificationAppService
    {
        Task<PaginatedResult<NotificationReadDto>> GetByUserAsync(
            int userId,
            PaginationParams pagination);

        Task<PaginatedResult<NotificationReadDto>> GetUnreadByUserAsync(
            int userId,
            PaginationParams pagination);

        Task<int> GetUnreadCountAsync(int userId);

        Task MarkAsReadAsync(int notificationId);
    }
}
