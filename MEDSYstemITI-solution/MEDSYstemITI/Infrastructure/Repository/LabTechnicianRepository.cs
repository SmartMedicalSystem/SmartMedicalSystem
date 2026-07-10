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
    /// Repository for managing LabTechnician entities.
    /// Provides CRUD operations and specialized queries for lab technicians.
    /// </summary>
    public class LabTechnicianRepository : GenericRepository<LabTechnician>, ILabTechnicianRepo
    {
        public LabTechnicianRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a lab technician by ID, excluding soft-deleted records.
        /// </summary>
        public override async Task<LabTechnician?> GetByIdAsync(int id)
        {
            return await _context.LabTechnicians
                .Where(lt => lt.Id == id && !lt.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active (non-deleted) lab technicians from the database.
        /// </summary>
        public override async Task<IEnumerable<LabTechnician>> GetAllAsync()
        {
            return await _context.LabTechnicians
                .Where(lt => !lt.IsDeleted)
                .OrderBy(lt => lt.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Searches for a lab technician by name (case-insensitive).
        /// </summary>
        public async Task<LabTechnician?> GetByNameAsync(string name)
        {
            return await _context.LabTechnicians
                .Where(lt => !lt.IsDeleted && lt.Name!.ToLower().Contains(name.ToLower()))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active lab technicians.
        /// Uses Skip and Take for pagination based on PageNumber and PageSize.
        /// Example: Page 2, Size 10 = Skip(10).Take(10)
        /// </summary>
        public override async Task<PaginatedResult<LabTechnician>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            // Get total count of active records
            var totalCount = await _context.LabTechnicians
                .Where(lt => !lt.IsDeleted)
                .CountAsync();

            // Get paginated records using Skip/Take
            var items = await _context.LabTechnicians
                .Where(lt => !lt.IsDeleted)
                .OrderBy(lt => lt.Name)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<LabTechnician>.Create(items, totalCount, pagination);
        }
    }
}
