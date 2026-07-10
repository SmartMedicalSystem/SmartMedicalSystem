using Application.DTOs.Notification;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface INotificationService
    {
        Task<NotificationReadDto> CreateAsync(NotificationCreateDto dto);
        Task<IEnumerable<NotificationReadDto>> GetUnreadByUserAsync(int userId);
        Task<PaginatedResult<NotificationReadDto>> GetByUserAsync(int userId, PaginationParams pagination);
        Task MarkAsReadAsync(int id);
    }
}
