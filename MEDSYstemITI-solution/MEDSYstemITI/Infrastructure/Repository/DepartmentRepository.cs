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
    /// Repository for managing Department entities.
    /// Includes eager loading of related Doctor collections.
    /// Standard CRUD (Add/Update/Delete/SoftDelete/Exists) and the alias
    /// GetAllActiveAsync/GetAllActivePaginatedAsync members are inherited
    /// unchanged from GenericRepository&lt;Department&gt;.
    /// </summary>
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepo
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a department by ID with all related doctors.
        /// Uses INCLUDE to load the Doctors collection to avoid N+1 queries.
        /// </summary>
        public override async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .Where(d => d.Id == id && !d.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active departments with their doctor collections.
        /// </summary>
        public override async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Searches for a department by name (case-insensitive).
        /// </summary>
        public async Task<Department?> GetByNameAsync(string name)
        {
            return await _context.Departments
                .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .Where(d => !d.IsDeleted && d.Name.ToLower().Contains(name.ToLower()))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a department with all its related doctors (full collection loading).
        /// Explicitly loads the Doctors collection for the specified department ID.
        /// </summary>
        public async Task<Department?> GetWithDoctorsAsync(int id)
        {
            var department = await _context.Departments
                .Where(d => d.Id == id && !d.IsDeleted)
                .FirstOrDefaultAsync();

            if (department != null)
            {
                // Explicitly load the Doctors collection
                await _context.Entry(department)
                    .Collection(d => d.Doctors)
                    .LoadAsync();
            }

            return department;
        }

        /// <summary>
        /// Retrieves a paginated list of all active departments.
        /// </summary>
        public override async Task<PaginatedResult<Department>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            // Get total count
            var totalCount = await _context.Departments
                .Where(d => !d.IsDeleted)
                .CountAsync();

            // Get paginated departments
            var items = await _context.Departments
                .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Department>.Create(items, totalCount, pagination);
        }
    }
}
