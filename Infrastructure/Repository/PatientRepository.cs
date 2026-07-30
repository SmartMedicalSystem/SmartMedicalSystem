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
    /// Repository for managing Patient entities.
    /// Includes eager loading of related Sessions.
    /// </summary>
    public class PatientRepository : GenericRepository<Patient>, IPatientRepo
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a patient by ID, excluding soft-deleted records.
        /// </summary>
        public override async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active patients from the database.
        /// </summary>
        public override async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all patients with their related sessions using INCLUDE.
        /// This eagerly loads the Sessions collection to avoid N+1 queries.
        /// </summary>
        public async Task<IEnumerable<Patient>> GetAllWithSessionsAsync()
        {
            return await _context.Patients
                .Include(p => p.Sessions.Where(s => !s.IsDeleted)) // INCLUDE sessions and filter active ones
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active patients.
        /// </summary>
        public override async Task<PaginatedResult<Patient>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.Patients
                .Where(p => !p.IsDeleted)
                .CountAsync();

            var items = await _context.Patients
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Patient>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves a paginated list of all patients with their sessions.
        /// </summary>
        public async Task<PaginatedResult<Patient>> GetAllWithSessionsPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.Patients
                .Where(p => !p.IsDeleted)
                .CountAsync();

            var items = await _context.Patients
                .Include(p => p.Sessions.Where(s => !s.IsDeleted))
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Patient>.Create(items, totalCount, pagination);
        }
    }
}
