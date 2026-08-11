using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.IRepository;
using Domain.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepo
    {
        public NotificationRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        private IQueryable<Notification> NotificationQuery()
        {
            return _context.Notifications

                // User
                .Include(n => n.User)

                // RequestLab -> Session -> Patient
                .Include(n => n.RequestLabs)
                    .ThenInclude(r => r!.Session)
                        .ThenInclude(s => s.Patient)

                // PatientResult -> Patient
                .Include(n => n.PatientResult)
                    .ThenInclude(r => r!.Patient)

                // PatientResult -> Session
                .Include(n => n.PatientResult)
                    .ThenInclude(r => r!.Session);
        }


        // ============================================================
        // Get By Id
        // ============================================================

        public override async Task<Notification?> GetByIdAsync(int id)
        {
            return await NotificationQuery()
                .FirstOrDefaultAsync(n => n.Id == id);
        }


        // ============================================================
        // Get All
        // ============================================================

        public override async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await NotificationQuery()
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }


        // ============================================================
        // Get By User
        // ============================================================

        public async Task<IEnumerable<Notification>> GetByUserAsync(
            int userId)
        {
            return await NotificationQuery()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }


        // ============================================================
        // Get Unread By User
        // ============================================================

        public async Task<IEnumerable<Notification>> GetUnreadByUserAsync(
            int userId)
        {
            return await NotificationQuery()
                .Where(n =>
                    n.UserId == userId &&
                    !n.IsRead)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }


        // ============================================================
        // Get All Paginated
        // ============================================================

        public override async Task<PaginatedResult<Notification>>
            GetAllPaginatedAsync(
                PaginationParams pagination)
        {
            var query = NotificationQuery();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(n => n.SentAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Notification>.Create(
                items,
                totalCount,
                pagination);
        }


        // ============================================================
        // Get By User Paginated
        // ============================================================

        public async Task<PaginatedResult<Notification>>
            GetByUserPaginatedAsync(
                int userId,
                PaginationParams pagination)
        {
            var query = NotificationQuery()
                .Where(n => n.UserId == userId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(n => n.SentAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Notification>.Create(
                items,
                totalCount,
                pagination);
        }


        // ============================================================
        // Get Unread By User Paginated
        // ============================================================

        public async Task<PaginatedResult<Notification>>
            GetUnreadByUserPaginatedAsync(
                int userId,
                PaginationParams pagination)
        {
            var query = NotificationQuery()
                .Where(n =>
                    n.UserId == userId &&
                    !n.IsRead);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(n => n.SentAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Notification>.Create(
                items,
                totalCount,
                pagination);
        }


        // ============================================================
        // Mark As Read
        // ============================================================

        public async Task<Notification> MarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                throw new InvalidOperationException(
                    $"Notification with ID {id} not found.");
            }

            notification.MarkAsRead();

            notification.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return notification;
        }


        // ============================================================
        // Get Unread Count
        // ============================================================

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n =>
                    n.UserId == userId &&
                    !n.IsRead);
        }
    }
}