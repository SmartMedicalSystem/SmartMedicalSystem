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
    /// Repository for managing Session entities.
    /// Demonstrates complex LINQ JOINs with multiple related entities: Patient, Doctor, and Department.
    /// GetByIdAsync is inherited as-is from GenericRepository&lt;Session&gt; since it has no
    /// extra Include/ordering beyond the base implementation.
    /// </summary>
    public class SessionRepository : GenericRepository<Session>, ISessionRepo
    {
        public SessionRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves all active sessions from the database.
        /// </summary>
        public override async Task<IEnumerable<Session>> GetAllAsync()
        {
            return await _context.Sessions
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all sessions for a specific patient using filtering by foreign key.
        /// </summary>
        public async Task<IEnumerable<Session>> GetByPatientAsync(int patientId)
        {
            return await _context.Sessions
                .Where(s => s.PatientId == patientId && !s.IsDeleted)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all sessions conducted by a specific doctor using filtering by foreign key.
        /// </summary>
        public async Task<IEnumerable<Session>> GetByDoctorAsync(int doctorId)
        {
            return await _context.Sessions
                .Where(s => s.DoctorId == doctorId && !s.IsDeleted)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all sessions in a specific department using filtering by foreign key.
        /// </summary>
        public async Task<IEnumerable<Session>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.Sessions
                .Where(s => s.DeptId == departmentId && !s.IsDeleted)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a session with all its related detail information.
        /// Uses multiple INCLUDE statements to perform JOINs with Patient, Doctor, and Department tables.
        /// This is a complex query demonstrating multi-table JOIN in EF Core LINQ.
        /// 
        /// Query Structure:
        /// - INCLUDE Patient (1:1 relationship)
        /// - INCLUDE Doctor (1:1 relationship) which includes its Department
        /// - INCLUDE Department (1:1 relationship)
        /// </summary>
        public async Task<Session?> GetWithDetailsAsync(int id)
        {
            return await _context.Sessions
                .Include(s => s.Patient) // JOIN with Patient table
                .Include(s => s.Doctor) // JOIN with Doctor table
                    .ThenInclude(d => d.Department) // Then JOIN Doctor's Department
                .Include(s => s.Department) // JOIN with Department table
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active sessions.
        /// </summary>
        public override async Task<PaginatedResult<Session>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.Sessions
                .Where(s => !s.IsDeleted)
                .CountAsync();

            var items = await _context.Sessions
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.SessionDate)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Session>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves sessions for a specific patient in paginated format.
        /// </summary>
        public async Task<PaginatedResult<Session>> GetByPatientPaginatedAsync(int patientId, PaginationParams pagination)
        {
            var totalCount = await _context.Sessions
                .Where(s => s.PatientId == patientId && !s.IsDeleted)
                .CountAsync();

            var items = await _context.Sessions
                .Where(s => s.PatientId == patientId && !s.IsDeleted)
                .OrderByDescending(s => s.SessionDate)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Session>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves sessions conducted by a specific doctor in paginated format.
        /// </summary>
        public async Task<PaginatedResult<Session>> GetByDoctorPaginatedAsync(int doctorId, PaginationParams pagination)
        {
            var totalCount = await _context.Sessions
                .Where(s => s.DoctorId == doctorId && !s.IsDeleted)
                .CountAsync();

            var items = await _context.Sessions
                .Where(s => s.DoctorId == doctorId && !s.IsDeleted)
                .OrderByDescending(s => s.SessionDate)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Session>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves sessions in a specific department in paginated format.
        /// </summary>
        public async Task<PaginatedResult<Session>> GetByDepartmentPaginatedAsync(int departmentId, PaginationParams pagination)
        {
            var totalCount = await _context.Sessions
                .Where(s => s.DeptId == departmentId && !s.IsDeleted)
                .CountAsync();

            var items = await _context.Sessions
                .Where(s => s.DeptId == departmentId && !s.IsDeleted)
                .OrderByDescending(s => s.SessionDate)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Session>.Create(items, totalCount, pagination);
        }
    }
}
