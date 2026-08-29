using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Filters;
using Domain.IRepository;
using Domain.Models;
using Infrastructure.Context;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Repository for managing Patient entities.
    /// Includes eager loading of related Sessions.
    /// </summary>
    public class PatientRepository : GenericRepository<Patient>, IPatientRepo
    {
        private readonly NationalIDEncryptionService _encryptionService;

        public PatientRepository(
            ApplicationDbContext context,
            NationalIDEncryptionService encryptionService)
            : base(context)
        {
            _encryptionService = encryptionService;
        }

        /// <summary>
        /// Retrieves a patient by ID, excluding soft-deleted records.
        /// </summary>
        public override async Task<Patient?> GetByIdAsync(int id)
        {
            var patient = await _context.Patients
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();

            if (patient != null)
            {
                DecryptNationalId(patient);
            }

            return patient;
        }

        /// <summary>
        /// Retrieves all active patients from the database.
        /// </summary>
        public override async Task<IEnumerable<Patient>> GetAllAsync()
        {
            var patients = await _context.Patients
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToListAsync();

            DecryptNationalIds(patients);

            return patients;
        }

        public async Task<PaginatedResult<Patient>> GetFilteredPaginatedAsync(
            PatientFilterParams filter)
        {
            // Filters that can be applied directly in SQL
            IQueryable<Patient> query =
                _context.Patients.Where(p => !p.IsDeleted);

            if (filter.Gender.HasValue)
            {
                query = query.Where(
                    p => p.Gender == filter.Gender.Value);
            }
            var today = DateTime.Today;

            if (filter.MinAge.HasValue)
            {
                var maxDateOfBirth =
                    today.AddYears(-filter.MinAge.Value);

                query = query.Where(
                    p => p.DateOfBirth <= maxDateOfBirth);
            }

            if (filter.MaxAge.HasValue)
            {
                var minDateOfBirth =
                    today.AddYears(-(filter.MaxAge.Value + 1));

                query = query.Where(
                    p => p.DateOfBirth > minDateOfBirth);
            }

            // No search
            if (string.IsNullOrWhiteSpace(filter.Search))
            {
                var sqlTotalCount = await query.CountAsync();

                var sqlItems = await query
                    .OrderByDescending(p => p.Id)
                    .Skip(filter.CalculateSkip())
                    .Take(filter.PageSize)
                    .ToListAsync();

                // Decrypt National ID before returning
                DecryptNationalIds(sqlItems);

                return PaginatedResult<Patient>.Create(
                    sqlItems,
                    sqlTotalCount,
                    filter);
            }

            // Search
            // NationalId is encrypted, so we need to decrypt it
            // in memory before searching.
            var search = filter.Search.Trim();

            var candidates = await query.ToListAsync();

            var filtered = candidates
                .Where(p =>
                    p.FirstName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                    || p.LastName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                    || $"{p.FirstName} {p.LastName}".Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                    // If NationalId is stored as plain text
                    || (!string.IsNullOrWhiteSpace(
                            p.EncryptedNationalId)
                        && p.EncryptedNationalId.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase))

                    // If NationalId is encrypted
                    || DecryptedNationalIdContains(
                        p,
                        search))
                .ToList();

            var totalCount = filtered.Count;

            var items = filtered
                .OrderByDescending(p => p.Id)
                .Skip(filter.CalculateSkip())
                .Take(filter.PageSize)
                .ToList();

            // Decrypt National ID before returning
            DecryptNationalIds(items);

            return PaginatedResult<Patient>.Create(
                items,
                totalCount,
                filter);
        }

        /// <summary>
        /// Checks whether the decrypted National ID contains
        /// the search text.
        /// </summary>
        private bool DecryptedNationalIdContains(
            Patient patient,
            string search)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                    patient.EncryptedNationalId))
                {
                    return false;
                }

                var decrypted =
                    _encryptionService.Decrypt(
                        patient.EncryptedNationalId);

                return !string.IsNullOrWhiteSpace(decrypted)
                    && decrypted.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // The value may already be plain text
                return false;
            }
        }

        /// <summary>
        /// Decrypts the National ID of one patient.
        /// The encrypted value in the database remains unchanged.
        /// </summary>
        private void DecryptNationalId(Patient patient)
        {
            if (string.IsNullOrWhiteSpace(patient.EncryptedNationalId))
            {
                patient.DecryptedNationalId = string.Empty;
                return;
            }

            // لو القيمة أصلاً مشفرة
            if (patient.EncryptedNationalId.StartsWith("CfDJ8"))
            {
                patient.DecryptedNationalId =
                    _encryptionService.Decrypt(
                        patient.EncryptedNationalId);

                return;
            }

            // لو القيمة أصلاً Plain Text
            patient.DecryptedNationalId =
                patient.EncryptedNationalId;
        }
        /// <summary>
        /// Decrypts the National ID for a collection of patients.
        /// </summary>
        private void DecryptNationalIds(
            IEnumerable<Patient> patients)
        {
            foreach (var patient in patients)
            {
                DecryptNationalId(patient);
            }
        }

        /// <summary>
        /// Retrieves all patients with their related sessions using INCLUDE.
        /// This eagerly loads the Sessions collection to avoid N+1 queries.
        /// </summary>
        public async Task<IEnumerable<Patient>> GetAllWithSessionsAsync()
        {
            var patients = await _context.Patients
                .Include(p =>
                    p.Sessions.Where(s => !s.IsDeleted))
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToListAsync();

            DecryptNationalIds(patients);

            return patients;
        }

        /// <summary>
        /// Retrieves a paginated list of all active patients.
        /// </summary>
        public override async Task<PaginatedResult<Patient>>
            GetAllPaginatedAsync(
                PaginationParams pagination)
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

            DecryptNationalIds(items);

            return PaginatedResult<Patient>.Create(
                items,
                totalCount,
                pagination);
        }

        /// <summary>
        /// Retrieves a paginated list of all patients with their sessions.
        /// </summary>
        public async Task<PaginatedResult<Patient>>
            GetAllWithSessionsPaginatedAsync(
                PaginationParams pagination)
        {
            var totalCount = await _context.Patients
                .Where(p => !p.IsDeleted)
                .CountAsync();

            var items = await _context.Patients
                .Include(p =>
                    p.Sessions.Where(s => !s.IsDeleted))
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            DecryptNationalIds(items);

            return PaginatedResult<Patient>.Create(
                items,
                totalCount,
                pagination);
        }
    }
}