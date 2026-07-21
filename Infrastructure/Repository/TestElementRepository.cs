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
    /// Repository for managing TestElement entities.
    /// Test elements are the individual components/parameters of lab tests (e.g., Hemoglobin, Blood Type).
    /// </summary>
    public class TestElementRepository : GenericRepository<TestElement>, ITestElementRepo
    {
        public TestElementRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a test element by ID, excluding soft-deleted records.
        /// </summary>
        public override async Task<TestElement?> GetByIdAsync(int id)
        {
            return await _context.TestElements
                .Where(te => te.Id == id && !te.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active test elements from the database.
        /// </summary>
        public override async Task<IEnumerable<TestElement>> GetAllAsync()
        {
            return await _context.TestElements
                .Where(te => !te.IsDeleted)
                .OrderBy(te => te.ElementName)
                .ToListAsync();
        }

        /// <summary>
        /// Searches for test elements by name (case-insensitive).
        /// </summary>
        public async Task<TestElement?> GetByNameAsync(string elementName)
        {
            return await _context.TestElements
                .Where(te => !te.IsDeleted && te.ElementName.ToLower().Contains(elementName.ToLower()))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active test elements.
        /// </summary>
        public override async Task<PaginatedResult<TestElement>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.TestElements
                .Where(te => !te.IsDeleted)
                .CountAsync();

            var items = await _context.TestElements
                .Where(te => !te.IsDeleted)
                .OrderBy(te => te.ElementName)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<TestElement>.Create(items, totalCount, pagination);
        }
    }
}
