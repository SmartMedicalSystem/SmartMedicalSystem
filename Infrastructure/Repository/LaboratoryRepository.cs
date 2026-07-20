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
    public class LaboratoryRepository : GenericRepository<Laboratory>, ILaboratoryRepo
    {
        public LaboratoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        
        public async Task<PaginatedResult<Laboratory>> GetAllPaginatedAsync(
            PaginationParams pagination,
            string? searchTerm = null,
            string? statusFilter = null)
        {
            var query = _context.Laboratories
                .Include(l => l.HeadTechnician)
                .Include(l => l.Department)
                .Include(l => l.LabTests.Where(lt => !lt.IsDeleted))
                .Include(l => l.LabTechnicians.Where(t => !t.IsDeleted))
                .Where(l => !l.IsDeleted)
                .AsQueryable();

            // Search Filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.Name.ToLower().Contains(search) ||
                    l.Location.ToLower().Contains(search));
            }

            // Status Filter
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(l => l.Status.ToString() == statusFilter);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(l => l.Name)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Laboratory>.Create(items, totalCount, pagination);
        }

        public async Task<IEnumerable<Laboratory>> GetActiveLaboratoriesAsync()
        {
            return await _context.Laboratories
                .Include(l => l.HeadTechnician)
                .Include(l => l.Department)
                .Where(l => !l.IsDeleted && l.Status == LabStatus.Active)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Laboratory>> GetAllWithDetailsAsync()
        {
            return await _context.Laboratories
                .Include(l => l.HeadTechnician)
                .Include(l => l.Department)
                .Include(l => l.LabTests.Where(lt => !lt.IsDeleted))
                .Include(l => l.LabTechnicians.Where(t => !t.IsDeleted))
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Laboratory>> GetForSelectAsync()
        {
            var laboratories = await _context.Laboratories
                .Where(l => !l.IsDeleted && l.Status == LabStatus.Active)
                .OrderBy(l => l.Name)
                .ToListAsync();

         
            return laboratories.Select(l => new Laboratory(
                l.Name,
                l.Location,
                l.Phone,
                l.Status,
                l.HeadTechnicianId,
                l.DepartmentId,
                l.Code,
                l.Specialty
            )
            {
                Id = l.Id,
                CreatedAt = l.CreatedAt
            });
        }

        // ============================================================
        // Get Laboratories by Specialty
        // ============================================================
        public async Task<IEnumerable<Laboratory>> GetLaboratoriesBySpecialtyAsync(string specialty)
        {
            return await _context.Laboratories
                .Where(l => l.Specialty == specialty && !l.IsDeleted)
                .ToListAsync();
        }

        public async Task<Laboratory?> GetLaboratoryWithDetailsAsync(int id)
        {
            return await _context.Laboratories
                .Include(l => l.HeadTechnician)
                .Include(l => l.Department)
                .Include(l => l.LabTests.Where(lt => !lt.IsDeleted))
                .Include(l => l.LabTechnicians.Where(t => !t.IsDeleted))
                .Where(l => l.Id == id && !l.IsDeleted)
                .FirstOrDefaultAsync();
        }


        // ============================================================
        // Get Total Tests Per Month for a Laboratory
        // ============================================================
        public async Task<int> GetTotalTestsPerMonthAsync(int labId)
        {
            var now = DateTime.Now;

            return await _context.LabTests
                .Where(lt => lt.LaboratoryId == labId
                             && !lt.IsDeleted
                             && lt.CreatedAt.Month == now.Month
                             && lt.CreatedAt.Year == now.Year)
                .CountAsync();
        }

        // ============================================================
        // Check if Code is Unique
        // ============================================================
        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            var query = _context.Laboratories
                .Where(l => l.Code == code && !l.IsDeleted);

            if (excludeId.HasValue)
                query = query.Where(l => l.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        // ============================================================
        // Check if Name is Unique
        // ============================================================
        public async Task<bool> IsLaboratoryNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Laboratories
                .Where(l => !l.IsDeleted && l.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
                query = query.Where(l => l.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

      
    
    }
}