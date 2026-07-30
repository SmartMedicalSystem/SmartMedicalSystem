using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface INotificationRepo : IGenericRepository<Notification>
    {
        // Entity-specific reads
        Task<IEnumerable<Notification>> GetByUserAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadByUserAsync(int userId);

        // Entity-specific reads with Pagination
        Task<PaginatedResult<Notification>> GetByUserPaginatedAsync(int userId, PaginationParams pagination);
        Task<PaginatedResult<Notification>> GetUnreadByUserPaginatedAsync(int userId, PaginationParams pagination);

        // Entity-specific update
        Task<Notification> MarkAsReadAsync(int id);
        Task<int> GetUnreadCountAsync(int userId);
    }
}
