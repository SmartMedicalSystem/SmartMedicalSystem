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
    /// Repository for managing Doctor entities.
    /// Includes complex JOIN operations with Department and filtering by specialization.
    /// Standard CRUD and GetAllActive* aliases are inherited from GenericRepository&lt;Doctor&gt;.
    /// </summary>
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepo
    {
        public DoctorRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a doctor by ID with department information using JOIN.
        /// </summary>
        public override async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Where(d => d.Id == id && !d.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active doctors with their department information.
        /// Uses INCLUDE to eagerly load related Department data to avoid N+1 queries.
        /// </summary>
        public override async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Searches for doctors by name (case-insensitive).
        /// </summary>
        public async Task<Doctor?> GetByNameAsync(string name)
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted && d.Name.ToLower().Contains(name.ToLower()))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all doctors assigned to a specific department using JOIN.
        /// Demonstrates filtering by foreign key.
        /// </summary>
        public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Where(d => d.DepartmentId == departmentId && !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all doctors with a specific specialization.
        /// Demonstrates filtering by attribute field.
        /// </summary>
        public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization)
        {
            return await _context.Doctors
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted && d.Specialization.ToLower().Contains(specialization.ToLower()))
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active doctors with their department information.
        /// Uses Skip/Take for pagination.
        /// </summary>
        public override async Task<PaginatedResult<Doctor>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            // Get total count of active doctors
            var totalCount = await _context.Doctors
                .Where(d => !d.IsDeleted)
                .CountAsync();

            // Get paginated doctors with department information
            var items = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Doctor>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves doctors assigned to a specific department in paginated format.
        /// </summary>
        public async Task<PaginatedResult<Doctor>> GetByDepartmentPaginatedAsync(int departmentId, PaginationParams pagination)
        {
            // Get total count
            var totalCount = await _context.Doctors
                .Where(d => d.DepartmentId == departmentId && !d.IsDeleted)
                .CountAsync();

            // Get paginated results
            var items = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => d.DepartmentId == departmentId && !d.IsDeleted)
                .OrderBy(d => d.Name)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Doctor>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves doctors with a specific specialization in paginated format.
        /// </summary>
        public async Task<PaginatedResult<Doctor>> GetBySpecializationPaginatedAsync(string specialization, PaginationParams pagination)
        {
            // Get total count
            var totalCount = await _context.Doctors
                .Where(d => !d.IsDeleted && d.Specialization.ToLower().Contains(specialization.ToLower()))
                .CountAsync();

            // Get paginated results
            var items = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted && d.Specialization.ToLower().Contains(specialization.ToLower()))
                .OrderBy(d => d.Name)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Doctor>.Create(items, totalCount, pagination);
        }
    }
}
