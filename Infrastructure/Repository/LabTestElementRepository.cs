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
    /// Repository for managing LabTestElement entities.
    /// This is a junction table repository that manages many-to-many relationship between LabTest and TestElement.
    /// Demonstrates handling of junction/bridge table operations with composite keys.
    /// </summary>
    public class LabTestElementRepository : ILabTestElementRepo
    {
        private readonly ApplicationDbContext _context;

        public LabTestElementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new association between a LabTest and a TestElement.
        /// Junction tables typically don't have separate CreatedAt/IsDeleted unless needed.
        /// </summary>
        public async Task<LabTestElement> AddAsync(LabTestElement labTestElement)
        {
            await _context.LabTestElements.AddAsync(labTestElement);
            await _context.SaveChangesAsync();

            return labTestElement;
        }

        /// <summary>
        /// Retrieves a specific association by composite key (LabTestId, TestElementId).
        /// Uses INCLUDE to load related LabTest and TestElement entities.
        /// </summary>
        public async Task<LabTestElement?> GetByIdAsync(int labTestId, int testElementId)
        {
            return await _context.LabTestElements
                .Include(lte => lte.LabTest)
                .Include(lte => lte.TestElement)
                .Where(lte => lte.LabTestId == labTestId && lte.TestElementId == testElementId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all test elements associated with a specific lab test.
        /// Uses foreign key filtering to find all elements in a particular lab test.
        /// Includes full TestElement details via JOIN.
        /// </summary>
        public async Task<IEnumerable<LabTestElement>> GetByLabTestAsync(int labTestId)
        {
            return await _context.LabTestElements
                .Include(lte => lte.TestElement) // JOIN with TestElement
                .Where(lte => lte.LabTestId == labTestId)
                .OrderBy(lte => lte.TestElementId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all lab tests that contain a specific test element.
        /// Demonstrates reverse lookup: find all lab tests using a particular element.
        /// Uses foreign key filtering and INCLUDE for LabTest details.
        /// </summary>
        public async Task<IEnumerable<LabTestElement>> GetByTestElementAsync(int testElementId)
        {
            return await _context.LabTestElements
                .Include(lte => lte.LabTest) // JOIN with LabTest
                .Where(lte => lte.TestElementId == testElementId)
                .OrderBy(lte => lte.LabTestId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all junction table records (all associations).
        /// Includes both LabTest and TestElement details.
        /// </summary>
        public async Task<IEnumerable<LabTestElement>> GetAllAsync()
        {
            return await _context.LabTestElements
                .Include(lte => lte.LabTest)
                .Include(lte => lte.TestElement)
                .OrderBy(lte => lte.LabTestId)
                .ThenBy(lte => lte.TestElementId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all junction table records.
        /// </summary>
        public async Task<PaginatedResult<LabTestElement>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.LabTestElements.CountAsync();

            var items = await _context.LabTestElements
                .Include(lte => lte.LabTest)
                .Include(lte => lte.TestElement)
                .OrderBy(lte => lte.LabTestId)
                .ThenBy(lte => lte.TestElementId)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<LabTestElement>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves test elements for a specific lab test in paginated format.
        /// </summary>
        public async Task<PaginatedResult<LabTestElement>> GetByLabTestPaginatedAsync(int labTestId, PaginationParams pagination)
        {
            var totalCount = await _context.LabTestElements
                .Where(lte => lte.LabTestId == labTestId)
                .CountAsync();

            var items = await _context.LabTestElements
                .Include(lte => lte.TestElement)
                .Where(lte => lte.LabTestId == labTestId)
                .OrderBy(lte => lte.TestElementId)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<LabTestElement>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves lab tests containing a specific test element in paginated format.
        /// </summary>
        public async Task<PaginatedResult<LabTestElement>> GetByTestElementPaginatedAsync(int testElementId, PaginationParams pagination)
        {
            var totalCount = await _context.LabTestElements
                .Where(lte => lte.TestElementId == testElementId)
                .CountAsync();

            var items = await _context.LabTestElements
                .Include(lte => lte.LabTest)
                .Where(lte => lte.TestElementId == testElementId)
                .OrderBy(lte => lte.LabTestId)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<LabTestElement>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Updates an existing junction table record.
        /// Since this is a junction table with no separate identity, updates are rare,
        /// but this method is included for completeness.
        /// </summary>
        public async Task<LabTestElement> UpdateAsync(LabTestElement labTestElement)
        {
            _context.LabTestElements.Update(labTestElement);
            await _context.SaveChangesAsync();

            return labTestElement;
        }

        /// <summary>
        /// Permanently deletes an association between a LabTest and TestElement by composite key.
        /// This removes the junction table record, breaking the many-to-many relationship.
        /// </summary>
        public async Task<bool> DeleteAsync(int labTestId, int testElementId)
        {
            var labTestElement = await _context.LabTestElements
                .FirstOrDefaultAsync(lte => lte.LabTestId == labTestId && 
                                           lte.TestElementId == testElementId);
            if (labTestElement == null)
                return false;

            _context.LabTestElements.Remove(labTestElement);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Checks if an association exists between a LabTest and TestElement.
        /// Returns true if the composite key exists in the junction table.
        /// </summary>
        public async Task<bool> ExistsAsync(int labTestId, int testElementId)
        {
            return await _context.LabTestElements
                .AnyAsync(lte => lte.LabTestId == labTestId && lte.TestElementId == testElementId);
        }
    }
}
