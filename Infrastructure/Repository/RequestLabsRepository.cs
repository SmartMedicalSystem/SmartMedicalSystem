using Domain.Entities;
using Domain.Enums;
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
    /// Repository for managing RequestLabs entities.
    /// Demonstrates filtering by Enum status and many-to-many relationship with LabTest.
    /// GetByIdAsync is inherited as-is from GenericRepository&lt;RequestLabs&gt; since it has no
    /// extra Include/ordering beyond the base implementation.
    /// </summary>
    public class RequestLabsRepository : GenericRepository<RequestLabs>, IRequestLabsRepo
    {
        public RequestLabsRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves all active lab requests from the database.
        /// </summary>
        public override async Task<IEnumerable<RequestLabs>> GetAllAsync()
        {
            return await _context.RequestLabs
                .Where(rl => !rl.IsDeleted)
                .OrderByDescending(rl => rl.RequestedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all lab requests for a specific session.
        /// Filters using the foreign key SessionId.
        /// </summary>
        public async Task<IEnumerable<RequestLabs>> GetBySessionAsync(int sessionId)
        {
            return await _context.RequestLabs
                .Where(rl => rl.SessionId == sessionId && !rl.IsDeleted)
                .OrderByDescending(rl => rl.RequestedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all lab requests with a specific status using Enum filtering.
        /// Demonstrates filtering by enumeration type (LabRequestStatus: Pending, Completed, etc).
        /// </summary>
        public async Task<IEnumerable<RequestLabs>> GetByStatusAsync(LabRequestStatus status)
        {
            return await _context.RequestLabs
                .Where(rl => rl.Status == status && !rl.IsDeleted)
                .OrderByDescending(rl => rl.RequestedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a lab request with all its related lab tests.
        /// Uses INCLUDE and ThenInclude to perform JOINs with many-to-many LabTests collection.
        /// 
        /// Query Structure:
        /// - INCLUDE LabTests (many-to-many relationship)
        /// 
        /// This shows how to load all tests associated with a lab request.
        /// </summary>
        public async Task<RequestLabs?> GetWithLabTestsAsync(int id)
        {
            return await _context.RequestLabs
                .Include(rl => rl.LabTests.Where(lt => !lt.IsDeleted)) // JOIN with LabTests and filter active ones
                .Where(rl => rl.Id == id && !rl.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active lab requests.
        /// </summary>
        public override async Task<PaginatedResult<RequestLabs>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.RequestLabs
                .Where(rl => !rl.IsDeleted)
                .CountAsync();

            var items = await _context.RequestLabs
                .Where(rl => !rl.IsDeleted)
                .OrderByDescending(rl => rl.RequestedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<RequestLabs>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves lab requests for a specific session in paginated format.
        /// </summary>
        public async Task<PaginatedResult<RequestLabs>> GetBySessionPaginatedAsync(int sessionId, PaginationParams pagination)
        {
            var totalCount = await _context.RequestLabs
                .Where(rl => rl.SessionId == sessionId && !rl.IsDeleted)
                .CountAsync();

            var items = await _context.RequestLabs
                .Where(rl => rl.SessionId == sessionId && !rl.IsDeleted)
                .OrderByDescending(rl => rl.RequestedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<RequestLabs>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves lab requests with a specific status in paginated format.
        /// </summary>
        public async Task<PaginatedResult<RequestLabs>> GetByStatusPaginatedAsync(LabRequestStatus status, PaginationParams pagination)
        {
            var totalCount = await _context.RequestLabs
                .Where(rl => rl.Status == status && !rl.IsDeleted)
                .CountAsync();

            var items = await _context.RequestLabs
                .Where(rl => rl.Status == status && !rl.IsDeleted)
                .OrderByDescending(rl => rl.RequestedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<RequestLabs>.Create(items, totalCount, pagination);
        }
    }
}
