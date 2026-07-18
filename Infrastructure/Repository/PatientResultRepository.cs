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
    /// Repository for managing PatientResult entities.
    /// Demonstrates complex multi-table JOINs with Patient, Session, LabTest, and related ResultElements.
    /// GetByIdAsync is inherited as-is from GenericRepository&lt;PatientResult&gt; since it has no
    /// extra Include/ordering beyond the base implementation.
    /// </summary>
    public class PatientResultRepository : GenericRepository<PatientResult>, IPatientResultRepo
    {
        public PatientResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves all active patient results from the database.
        /// </summary>
        public override async Task<IEnumerable<PatientResult>> GetAllAsync()
        {
            return await _context.PatientResults
                .Where(pr => !pr.IsDeleted)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all results for a specific patient using foreign key filtering.
        /// </summary>
        public async Task<IEnumerable<PatientResult>> GetByPatientAsync(int patientId)
        {
            return await _context.PatientResults
                .Where(pr => pr.PatientId == patientId && !pr.IsDeleted)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all results for a specific session.
        /// </summary>
        public async Task<IEnumerable<PatientResult>> GetBySessionAsync(int sessionId)
        {
            return await _context.PatientResults
                .Where(pr => pr.SessionId == sessionId && !pr.IsDeleted)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all results for a specific lab test.
        /// </summary>
        public async Task<IEnumerable<PatientResult>> GetByLabTestAsync(int labTestId)
        {
            return await _context.PatientResults
                .Where(pr => pr.LabTestId == labTestId && !pr.IsDeleted)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a patient result with all its related result elements and referenced entities.
        /// Uses complex LINQ JOINs demonstrating:
        /// - Include Patient (1:1 relationship)
        /// - Include Session (1:1 relationship)
        /// - Include LabTest (1:1 relationship)
        /// - Include ResultElements with ThenInclude for TestElement and Technician (1:many with nested 1:1)
        /// 
        /// Query Structure:
        /// PatientResult
        ///   ├─ Patient
        ///   ├─ Session
        ///   ├─ LabTest
        ///   └─ ResultElements[] (1:many)
        ///       ├─ TestElement
        ///       └─ Technician (LabTechnician)
        /// </summary>
        public async Task<PatientResult?> GetWithResultElementsAsync(int id)
        {
            return await _context.PatientResults
                .Include(pr => pr.Patient) // JOIN with Patient table
                .Include(pr => pr.Session) // JOIN with Session table
                .Include(pr => pr.labTest) // JOIN with LabTest table
                .Include(pr => pr.ResultElements) // JOIN with PatientResultElement collection
                    .ThenInclude(pre => pre.TestElement) // Then JOIN TestElement for each ResultElement
                .Include(pr => pr.ResultElements) // Re-include to load Technician
                    .ThenInclude(pre => pre.Technician) // Then JOIN LabTechnician for each ResultElement
                .Where(pr => pr.Id == id && !pr.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active patient results.
        /// </summary>
        public override async Task<PaginatedResult<PatientResult>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.PatientResults
                .Where(pr => !pr.IsDeleted)
                .CountAsync();

            var items = await _context.PatientResults
                .Where(pr => !pr.IsDeleted)
                .OrderByDescending(pr => pr.CreatedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<PatientResult>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves patient results for a specific patient in paginated format.
        /// </summary>
        public async Task<PaginatedResult<PatientResult>> GetByPatientPaginatedAsync(int patientId, PaginationParams pagination)
        {
            var totalCount = await _context.PatientResults
                .Where(pr => pr.PatientId == patientId && !pr.IsDeleted)
                .CountAsync();

            var items = await _context.PatientResults
                .Where(pr => pr.PatientId == patientId && !pr.IsDeleted)
                .OrderByDescending(pr => pr.CreatedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<PatientResult>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves patient results for a specific session in paginated format.
        /// </summary>
        public async Task<PaginatedResult<PatientResult>> GetBySessionPaginatedAsync(int sessionId, PaginationParams pagination)
        {
            var totalCount = await _context.PatientResults
                .Where(pr => pr.SessionId == sessionId && !pr.IsDeleted)
                .CountAsync();

            var items = await _context.PatientResults
                .Where(pr => pr.SessionId == sessionId && !pr.IsDeleted)
                .OrderByDescending(pr => pr.CreatedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<PatientResult>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves patient results for a specific lab test in paginated format.
        /// </summary>
        public async Task<PaginatedResult<PatientResult>> GetByLabTestPaginatedAsync(int labTestId, PaginationParams pagination)
        {
            var totalCount = await _context.PatientResults
                .Where(pr => pr.LabTestId == labTestId && !pr.IsDeleted)
                .CountAsync();

            var items = await _context.PatientResults
                .Where(pr => pr.LabTestId == labTestId && !pr.IsDeleted)
                .OrderByDescending(pr => pr.CreatedAt)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<PatientResult>.Create(items, totalCount, pagination);
        }
    }
}
