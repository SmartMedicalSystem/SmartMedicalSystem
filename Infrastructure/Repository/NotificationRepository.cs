using Domain.Entities;
using Domain.IRepository;
using Domain.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Repository for managing Notification entities.
    /// Includes filtering by user and read/unread status, with JOIN to ApplicationUser.
    /// </summary>
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepo
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a notification by ID, excluding soft-deleted records.
        /// </summary>
        public override async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications
                .Include(n => n.User) // JOIN with ApplicationUser
                .Where(n => n.Id == id && !n.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active notifications from the database.
        /// </summary>
        public override async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Where(n => !n.IsDeleted)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all notifications for a specific user using foreign key filtering.
        /// Uses INCLUDE to load the related ApplicationUser data.
        /// </summary>
        public async Task<IEnumerable<Notification>> GetByUserAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.User) // JOIN with ApplicationUser
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all unread notifications for a specific user.
        /// Demonstrates filtering by multiple criteria: UserId, IsRead status, and IsDeleted.
        /// This query would typically be used for notification badges or notification centers.
        /// </summary>
        public async Task<IEnumerable<Notification>> GetUnreadByUserAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.User) // JOIN with ApplicationUser
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active notifications.
        /// </summary>
        public override async Task<PaginatedResult<Notification>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.Notifications
                .Where(n => !n.IsDeleted)
                .CountAsync();

            var items = await _context.Notifications
                .Include(n => n.User)
                .Where(n => !n.IsDeleted)
                .OrderByDescending(n => n.SentAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Notification>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves notifications for a specific user in paginated format.
        /// </summary>
        public async Task<PaginatedResult<Notification>> GetByUserPaginatedAsync(int userId, PaginationParams pagination)
        {
            var totalCount = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .CountAsync();

            var items = await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.SentAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Notification>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves unread notifications for a specific user in paginated format.
        /// </summary>
        public async Task<PaginatedResult<Notification>> GetUnreadByUserPaginatedAsync(int userId, PaginationParams pagination)
        {
            var totalCount = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .CountAsync();

            var items = await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .OrderByDescending(n => n.SentAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Notification>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Marks a notification as read by setting IsRead = true and updating timestamp.
        /// Returns the updated notification object.
        /// </summary>
        public async Task<Notification> MarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                throw new InvalidOperationException($"Notification with ID {id} not found.");

            notification.MarkAsRead();
            notification.UpdatedAt = DateTime.UtcNow;

            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();

            return notification;
        }
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(x =>
                    x.UserId == userId &&
                    !x.IsRead &&
                    !x.IsDeleted);
        }
    }
}
