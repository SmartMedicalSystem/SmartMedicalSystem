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
    /// Repository for managing LabTest entities.
    /// Includes eager loading of related TestElements through LabTestElement junction table.
    /// </summary>
    public class LabTestRepository : GenericRepository<LabTest>, ILabTestRepo
    {
        public LabTestRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a lab test by ID, excluding soft-deleted records.
        /// </summary>
        public override async Task<LabTest?> GetByIdAsync(int id)
        {
            return await _context.LabTests
                .Where(lt => lt.Id == id && !lt.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active lab tests from the database.
        /// </summary>
        public override async Task<IEnumerable<LabTest>> GetAllAsync()
        {
            return await _context.LabTests
                .Where(lt => !lt.IsDeleted)
                .OrderBy(lt => lt.TestName)
                .ToListAsync();
        }

        /// <summary>
        /// Searches for a lab test by name (case-insensitive).
        /// </summary>
        public async Task<LabTest?> GetByNameAsync(string testName)
        {
            return await _context.LabTests
                .Where(lt => !lt.IsDeleted && lt.TestName.ToLower().Contains(testName.ToLower()))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a lab test with all its related TestElements.
        /// Uses INCLUDE with JOIN through LabTestElement junction table.
        /// This demonstrates a many-to-many relationship with explicit joining.
        /// </summary>
        public async Task<LabTest?> GetWithElementsAsync(int id)
        {
            return await _context.LabTests
                .Include(lt => lt.LabTestElements) // Load junction table records
                .ThenInclude(lte => lte.TestElement) // Then load related TestElements
                .Where(lt => lt.Id == id && !lt.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active lab tests.
        /// </summary>
        public override async Task<PaginatedResult<LabTest>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.LabTests
                .Where(lt => !lt.IsDeleted)
                .CountAsync();

            var items = await _context.LabTests
                .Where(lt => !lt.IsDeleted)
                .OrderBy(lt => lt.TestName)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<LabTest>.Create(items, totalCount, pagination);
        }
    }
}
