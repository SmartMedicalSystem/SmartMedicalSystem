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
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepo
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        // ============================================================
        // 1. GET BY ID with Doctors + Head Doctor
        // ============================================================
        public async Task<Department?> GetDepartmentWithDoctorsAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .Include(d => d.HeadDoctorEntity)
                .Where(d => d.Id == id && !d.IsDeleted)
                .FirstOrDefaultAsync();
        }

        // ============================================================
        // 2. GET ALL with Doctors + Head Doctor
        // ============================================================
        public async Task<IEnumerable<Department>> GetAllWithDoctorsAsync()
        {
            return await _context.Departments
                .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .Include(d => d.HeadDoctorEntity)
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        // ============================================================
        // 3. GET ALL PAGINATED with Doctors + Head Doctor
        // ============================================================
        // في DepartmentRepository
        public async Task<PaginatedResult<Department>> GetAllPaginatedAsync(
            PaginationParams pagination,
            string? searchTerm = null,
            string? statusFilter = null,
            string? creationDateFilter = null,
            string? managerFilter = null)
        {
            var query = _context.Departments
                .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .Include(d => d.HeadDoctorEntity)
                .Where(d => !d.IsDeleted)
                .AsQueryable();

            // Search Filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(search) ||
                                         d.HeadDoctor.ToLower().Contains(search));
            }

            // Status Filter
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(d => d.Status == statusFilter);
            }

            // Creation Date Filter
            if (!string.IsNullOrWhiteSpace(creationDateFilter))
            {
                var today = DateTime.Today;
                if (creationDateFilter == "Last 30 Days")
                {
                    var thirtyDaysAgo = today.AddDays(-30);
                    query = query.Where(d => d.CreatedAt >= thirtyDaysAgo);
                }
                else if (creationDateFilter == "This Year")
                {
                    var startOfYear = new DateTime(today.Year, 1, 1);
                    query = query.Where(d => d.CreatedAt >= startOfYear);
                }
            }

            // Manager Filter (Head Doctor)
            if (!string.IsNullOrWhiteSpace(managerFilter))
            {
                query = query.Where(d => d.HeadDoctor == managerFilter);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(d => d.Name)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Department>.Create(items, totalCount, pagination);
        }

        // ============================================================
        // 4. GET ACTIVE DEPARTMENTS
        // ============================================================
        public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
        {
            return await _context.Departments
                .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .Include(d => d.HeadDoctorEntity)
                .Where(d => !d.IsDeleted && d.Status == "Active")
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        // ============================================================
        // 5. CHECK UNIQUE DEPARTMENT NAME
        // ============================================================
        public async Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Departments
                .Where(d => !d.IsDeleted && d.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        // ============================================================
        // 6. GET BY NAME (Search)
        // ============================================================
        public async Task<Department?> GetByNameAsync(string name)
        {
            return await _context.Departments
                .Include(d => d.Doctors.Where(doc => !doc.IsDeleted))
                .Include(d => d.HeadDoctorEntity)
                .Where(d => !d.IsDeleted && d.Name.ToLower().Contains(name.ToLower()))
                .FirstOrDefaultAsync();
        }
    }
}