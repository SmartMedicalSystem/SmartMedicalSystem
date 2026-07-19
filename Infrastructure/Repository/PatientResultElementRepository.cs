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
    /// Repository for managing PatientResultElement entities.
    /// Represents individual test element results within a patient result.
    /// Demonstrates filtering by multiple foreign keys and complex relationship navigation.
    ///
    /// NOTE: PatientResultElement has its own surrogate Id (it derives from BaseEntity), so the
    /// single-id GetByIdAsync/DeleteAsync/SoftDeleteAsync/ExistsAsync members required by
    /// IGenericRepository&lt;PatientResultElement&gt; are now served by GenericRepository's real
    /// implementation instead of the NotImplementedException stubs that existed in the original
    /// hand-written repository. The composite-key (PatientResultId, TestElementId) lookup helpers
    /// below are kept as additional overloads for convenience, unchanged from the original code.
    /// </summary>
    public class PatientResultElementRepository : GenericRepository<PatientResultElement>, IPatientResultElementRepo
    {
        public PatientResultElementRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a patient result element by composite key (PatientResultId and TestElementId).
        /// </summary>
        public async Task<PatientResultElement?> GetByIdAsync(int patientResultId, int testElementId)
        {
            return await _context.PatientResultElements
                .Include(pre => pre.TestElement)
                .Include(pre => pre.Technician)
                .Where(pre => pre.PatientResultId == patientResultId &&
                             pre.TestElementId == testElementId &&
                             !pre.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all result elements for a specific patient result.
        /// Uses INCLUDE to load related TestElement and Technician data.
        /// </summary>
        public async Task<IEnumerable<PatientResultElement>> GetByPatientResultAsync(int patientResultId)
        {
            return await _context.PatientResultElements
                .Include(pre => pre.TestElement)
                .Include(pre => pre.Technician)
                .Where(pre => pre.PatientResultId == patientResultId && !pre.IsDeleted)
                .OrderBy(pre => pre.TestElementId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all result elements for a specific test element.
        /// Useful for finding all occurrences of a particular test result across patients.
        /// Uses foreign key filtering.
        /// </summary>
        public async Task<IEnumerable<PatientResultElement>> GetByTestElementAsync(int testElementId)
        {
            return await _context.PatientResultElements
                .Include(pre => pre.patientResult) // JOIN with PatientResult
                .Include(pre => pre.Technician) // JOIN with LabTechnician
                .Where(pre => pre.TestElementId == testElementId && !pre.IsDeleted)
                .OrderByDescending(pre => pre.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all result elements recorded by a specific technician.
        /// Demonstrates filtering by technician foreign key and includes related data.
        /// </summary>
        public async Task<IEnumerable<PatientResultElement>> GetByTechnicianAsync(int technicianId)
        {
            return await _context.PatientResultElements
                .Include(pre => pre.patientResult) // JOIN with PatientResult
                .Include(pre => pre.TestElement) // JOIN with TestElement
                .Where(pre => pre.TechId == technicianId && !pre.IsDeleted)
                .OrderByDescending(pre => pre.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all active patient result elements.
        /// </summary>
        public override async Task<IEnumerable<PatientResultElement>> GetAllAsync()
        {
            return await _context.PatientResultElements
                .Include(pre => pre.patientResult)
                .Include(pre => pre.TestElement)
                .Include(pre => pre.Technician)
                .Where(pre => !pre.IsDeleted)
                .OrderByDescending(pre => pre.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active patient result elements.
        /// </summary>
        public override async Task<PaginatedResult<PatientResultElement>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.PatientResultElements
                .Where(pre => !pre.IsDeleted)
                .CountAsync();

            var items = await _context.PatientResultElements
                .Include(pre => pre.patientResult)
                .Include(pre => pre.TestElement)
                .Include(pre => pre.Technician)
                .Where(pre => !pre.IsDeleted)
                .OrderByDescending(pre => pre.CreatedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<PatientResultElement>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves result elements for a specific patient result in paginated format.
        /// </summary>
        public async Task<PaginatedResult<PatientResultElement>> GetByPatientResultPaginatedAsync(int patientResultId, PaginationParams pagination)
        {
            var totalCount = await _context.PatientResultElements
                .Where(pre => pre.PatientResultId == patientResultId && !pre.IsDeleted)
                .CountAsync();

            var items = await _context.PatientResultElements
                .Include(pre => pre.TestElement)
                .Include(pre => pre.Technician)
                .Where(pre => pre.PatientResultId == patientResultId && !pre.IsDeleted)
                .OrderBy(pre => pre.TestElementId)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<PatientResultElement>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves result elements for a specific test element in paginated format.
        /// </summary>
        public async Task<PaginatedResult<PatientResultElement>> GetByTestElementPaginatedAsync(int testElementId, PaginationParams pagination)
        {
            var totalCount = await _context.PatientResultElements
                .Where(pre => pre.TestElementId == testElementId && !pre.IsDeleted)
                .CountAsync();

            var items = await _context.PatientResultElements
                .Include(pre => pre.patientResult)
                .Include(pre => pre.Technician)
                .Where(pre => pre.TestElementId == testElementId && !pre.IsDeleted)
                .OrderByDescending(pre => pre.CreatedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<PatientResultElement>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves result elements recorded by a specific technician in paginated format.
        /// </summary>
        public async Task<PaginatedResult<PatientResultElement>> GetByTechnicianPaginatedAsync(int technicianId, PaginationParams pagination)
        {
            var totalCount = await _context.PatientResultElements
                .Where(pre => pre.TechId == technicianId && !pre.IsDeleted)
                .CountAsync();

            var items = await _context.PatientResultElements
                .Include(pre => pre.patientResult)
                .Include(pre => pre.TestElement)
                .Where(pre => pre.TechId == technicianId && !pre.IsDeleted)
                .OrderByDescending(pre => pre.CreatedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<PatientResultElement>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Permanently deletes a patient result element by composite key.
        /// </summary>
        public async Task<bool> DeleteAsync(int patientResultId, int testElementId)
        {
            var element = await _context.PatientResultElements
                .FirstOrDefaultAsync(pre => pre.PatientResultId == patientResultId &&
                                           pre.TestElementId == testElementId);
            if (element == null)
                return false;

            _context.PatientResultElements.Remove(element);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Soft deletes a patient result element by composite key using IsDeleted flag.
        /// </summary>
        public async Task<bool> SoftDeleteAsync(int patientResultId, int testElementId)
        {
            var element = await _context.PatientResultElements
                .FirstOrDefaultAsync(pre => pre.PatientResultId == patientResultId &&
                                           pre.TestElementId == testElementId);
            if (element == null)
                return false;

            element.IsDeleted = true;
            element.UpdatedAt = DateTime.UtcNow;

            _context.PatientResultElements.Update(element);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Checks if a patient result element exists by composite key (active records only).
        /// </summary>
        public async Task<bool> ExistsAsync(int patientResultId, int testElementId)
        {
            return await _context.PatientResultElements
                .AnyAsync(pre => pre.PatientResultId == patientResultId &&
                                pre.TestElementId == testElementId &&
                                !pre.IsDeleted);
        }
    }
}
