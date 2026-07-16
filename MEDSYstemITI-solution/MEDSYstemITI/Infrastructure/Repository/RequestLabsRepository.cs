using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
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
        /// </summary>
        public async Task<IEnumerable<RequestLabs>> GetByStatusAsync(LabRequestStatus status)
        {
            return await _context.RequestLabs
                .Where(rl => rl.Status == status && !rl.IsDeleted)
                .OrderByDescending(rl => rl.RequestedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a lab request with all its related data needed for the read DTO:
        /// LabTests (many-to-many), and Session -> Patient / Doctor -> Department
        /// (needed for Patient name/SSN and "Requested By" doctor/department columns).
        /// </summary>
        public async Task<RequestLabs?> GetWithLabTestsAsync(int id)
        {
            return await _context.RequestLabs
                .Include(rl => rl.LabTests.Where(lt => !lt.IsDeleted))
                .Include(rl => rl.Session).ThenInclude(s => s.Patient)
                .Include(rl => rl.Session).ThenInclude(s => s.Doctor).ThenInclude(d => d.Department)
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

        /// <summary>
        /// Powers the dashboard grid: applies Status / Priority / LabTest / Doctor filters
        /// plus a free-text search across patient name, SSN (NationalId) and doctor name,
        /// then paginates the result. Includes everything the read DTO needs so no
        /// extra round-trips are required by the mapper.
        /// </summary>
        public async Task<PaginatedResult<RequestLabs>> GetFilteredAsync(RequestLabsFilterParams filter, PaginationParams pagination)
        {
            var query = _context.RequestLabs
                .Include(rl => rl.LabTests.Where(lt => !lt.IsDeleted))
                .Include(rl => rl.Session).ThenInclude(s => s.Patient)
                .Include(rl => rl.Session).ThenInclude(s => s.Doctor).ThenInclude(d => d.Department)
                .Where(rl => !rl.IsDeleted)
                .AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(rl => rl.Status == filter.Status.Value);

            if (filter.Priority.HasValue)
                query = query.Where(rl => rl.Priority == filter.Priority.Value);

            if (filter.LabTestId.HasValue)
                query = query.Where(rl => rl.LabTests.Any(lt => lt.Id == filter.LabTestId.Value));

            if (filter.DoctorId.HasValue)
                query = query.Where(rl => rl.Session.DoctorId == filter.DoctorId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                query = query.Where(rl =>
                    (rl.Session.Patient.FirstName + " " + rl.Session.Patient.LastName).Contains(term) ||
                    rl.Session.Patient.NationalId.ToString().Contains(term) ||
                    rl.Session.Doctor.Name.Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(rl => rl.RequestedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<RequestLabs>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Powers the 3 stat cards: Total Requests / Pending / Completed Today.
        /// "Completed Today" checks UpdatedAt (set whenever UpdateStatus/SoftDelete runs)
        /// against today's date in UTC.
        /// </summary>
        public async Task<RequestLabsStats> GetStatsAsync()
        {
            var today = DateTime.UtcNow.Date;

            var totalRequests = await _context.RequestLabs.CountAsync(rl => !rl.IsDeleted);

            var pending = await _context.RequestLabs
                .CountAsync(rl => !rl.IsDeleted && rl.Status == LabRequestStatus.Pending);

            var completedToday = await _context.RequestLabs
                .CountAsync(rl => !rl.IsDeleted
                    && rl.Status == LabRequestStatus.Completed
                    && rl.UpdatedAt != null
                    && rl.UpdatedAt.Value.Date == today);

            return new RequestLabsStats(totalRequests, pending, completedToday);
        }
    }
}