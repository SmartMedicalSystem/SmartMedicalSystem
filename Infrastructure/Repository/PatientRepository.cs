using Domain.Entities;
using Domain.Filters;
using Domain.IRepository;
using Domain.Models;
using Infrastructure.Context;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Repository for managing Patient entities.
    /// Includes eager loading of related Sessions.
    /// </summary>
    public class PatientRepository : GenericRepository<Patient>, IPatientRepo
    {
        private readonly NationalIDEncryptionService _encryptionService;

        public PatientRepository(ApplicationDbContext context, NationalIDEncryptionService encryptionService)
            : base(context)
        {
            _encryptionService = encryptionService;
        }


        /// <summary>
        /// Retrieves a patient by ID, excluding soft-deleted records.
        /// </summary>
        public override async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all active patients from the database.
        /// </summary>
        public override async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<PaginatedResult<Patient>> GetFilteredPaginatedAsync(PatientFilterParams filter)
        {
            // الفلاتر اللي ممكن تتطبق على مستوى الـ SQL عادي (مش محتاجة فك تشفير)
            IQueryable<Patient> query = _context.Patients.Where(p => !p.IsDeleted);

            if (filter.Gender.HasValue)
                query = query.Where(p => p.Gender == filter.Gender.Value);

            if (filter.MinAge.HasValue)
                query = query.Where(p => p.Age >= filter.MinAge.Value);

            if (filter.MaxAge.HasValue)
                query = query.Where(p => p.Age <= filter.MaxAge.Value);

            // لو مفيش Search، كمّل بنفس منطق SQL العادي (أسرع، مفيش داعي نجيب كل حاجة للميموري)
            if (string.IsNullOrWhiteSpace(filter.Search))
            {
                var sqlTotalCount = await query.CountAsync();

                var sqlItems = await query
                    .OrderByDescending(p => p.Id)
                    .Skip(filter.CalculateSkip())
                    .Take(filter.PageSize)
                    .ToListAsync();

                return PaginatedResult<Patient>.Create(sqlItems, sqlTotalCount, filter);
            }

            // فيه Search: الـ NationalId مشفّر فمينفعش نستخدم LIKE/Contains عليه في
            // الـ SQL مباشرة - لازم نجيب الداتا (بعد فلاتر Gender/Age) ونفك
            // التشفير في الميموري عشان نقدر نقارن.
            var search = filter.Search.Trim().ToLower();

            var candidates = await query.ToListAsync();

            var filtered = candidates.Where(p =>
    p.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
    p.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
    $"{p.FirstName} {p.LastName}".Contains(search, StringComparison.OrdinalIgnoreCase) ||

    // لو القيمة مخزنة بدون تشفير
    p.EncryptedNationalId.Contains(search, StringComparison.OrdinalIgnoreCase) ||

    // ولو كانت مشفرة
    DecryptedNationalIdContains(p, search)
).ToList();

            var totalCount = filtered.Count;

            var items = filtered
                .OrderByDescending(p => p.Id)
                .Skip(filter.CalculateSkip())
                .Take(filter.PageSize)
                .ToList();

            return PaginatedResult<Patient>.Create(items, totalCount, filter);
        }

        private bool DecryptedNationalIdContains(Patient patient, string search)
        {
            try
            {
                var decrypted = _encryptionService.Decrypt(patient.EncryptedNationalId);

                return decrypted.Contains(search, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // غالبًا القيمة مش متشفرة
                return false;
            }
        }

        /// <summary>
        /// Retrieves all patients with their related sessions using INCLUDE.
        /// This eagerly loads the Sessions collection to avoid N+1 queries.
        /// </summary>
        public async Task<IEnumerable<Patient>> GetAllWithSessionsAsync()
        {
            return await _context.Patients
                .Include(p => p.Sessions.Where(s => !s.IsDeleted)) // INCLUDE sessions and filter active ones
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of all active patients.
        /// </summary>
        public override async Task<PaginatedResult<Patient>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.Patients
                .Where(p => !p.IsDeleted)
                .CountAsync();

            var items = await _context.Patients
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Patient>.Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Retrieves a paginated list of all patients with their sessions.
        /// </summary>
        public async Task<PaginatedResult<Patient>> GetAllWithSessionsPaginatedAsync(PaginationParams pagination)
        {
            var totalCount = await _context.Patients
                .Where(p => !p.IsDeleted)
                .CountAsync();

            var items = await _context.Patients
                .Include(p => p.Sessions.Where(s => !s.IsDeleted))
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<Patient>.Create(items, totalCount, pagination);
        }
    }
}
