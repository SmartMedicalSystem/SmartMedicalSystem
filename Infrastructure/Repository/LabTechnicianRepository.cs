using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class LabTechnicianRepository : GenericRepository<LabTechnician>, ILabTechnicianRepo
    {
        public LabTechnicianRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public override async Task<LabTechnician?> GetByIdAsync(int id)
        {
            return await _context.LabTechnicians
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public override async Task<IEnumerable<LabTechnician>> GetAllAsync()
        {
            return await _context.LabTechnicians
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        //public async Task<LabTechnician?> GetByEmployeeIdAsync(string employeeId)
        //{
        //    return await _context.LabTechnicians
        //        .FirstOrDefaultAsync(x =>
        //            !x.IsDeleted &&
        //            x.EmployeeId == employeeId);
        //}

        public async Task<LabTechnician?> GetByNationalIdAsync(string nationalId)
        {
            return await _context.LabTechnicians
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.EncryptedNationalId == nationalId);
        }

        public async Task<PaginatedResult<LabTechnician>> SearchAsync(
        string? search,
        string? laboratory,
        EmploymentStatus? employmentStatus,
        WorkShift? workShift,
        DateOnly? joiningDate,
        PaginationParams pagination)
        {
            var query = _context.LabTechnicians
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(x =>
                    x.FirstName.ToLower().Contains(search) ||
                    x.LastName.ToLower().Contains(search) ||
                    //x.EmployeeId.ToLower().Contains(search) ||
                    x.PhoneNumber.ToLower().Contains(search) ||
                    x.Email.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(laboratory))
            {
                //query = query.Where(x => x.LaboratoryId == );
                // connect labortary with labtechnician with laboratory name where labtech include labid
                query = query.Where(x => x.Laboratory.Name.ToLower() == laboratory.Trim().ToLower());
            }

            if (employmentStatus.HasValue)
            {
                query = query.Where(x => x.EmploymentStatus == employmentStatus.Value);
            }

            if (workShift.HasValue)
            {
                query = query.Where(x => x.WorkShift == workShift.Value);
            }

            if (joiningDate.HasValue)
            {
                query = query.Where(x => x.JoiningDate == joiningDate.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<LabTechnician>.Create(items, totalCount, pagination);
        }

        // ===== NEW: Get technicians by Laboratory with Pagination =====
        public async Task<PaginatedResult<LabTechnician>> GetByLaboratoryIdAsync(
            int laboratoryId,
            PaginationParams pagination,
            string? searchTerm = null)
        {
            var query = _context.LabTechnicians
                .Where(t => !t.IsDeleted && t.LaboratoryId == laboratoryId)
                .Include(t => t.Laboratory)
                .AsQueryable();

            // Search by Name or JobTitle
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(t =>
                    (t.FirstName + " " + t.LastName).ToLower().Contains(search) ||
                    t.JobTitle.ToLower().Contains(search) ||
                    t.Username.ToLower().Contains(search)
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(t => t.FirstName)
                .ThenBy(t => t.LastName)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<LabTechnician>.Create(items, totalCount, pagination);
        }

        public async Task<PaginatedResult<LabTechnician>>
       GetAvailableForLaboratoryAsync(
           int laboratoryId,
           PaginationParams pagination,
           string? searchTerm = null)
        {
            var query =
                _context.LabTechnicians
                    .Where(t =>
                        !t.IsDeleted &&
                        (
                            t.LaboratoryId == laboratoryId
                            ||
                            !_context.Laboratories.Any(l =>
                                l.HeadTechnicianId == t.Id &&
                                !l.IsDeleted)
                        ))
                    .Include(t => t.Laboratory)
                    .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search =
                    searchTerm.Trim().ToLower();

                query = query.Where(t =>
                    (t.FirstName + " " + t.LastName)
                        .ToLower()
                        .Contains(search)
                    ||
                    t.JobTitle
                        .ToLower()
                        .Contains(search));
            }

            var totalCount =
                await query.CountAsync();

            var items =
                await query
                    .OrderBy(t => t.FirstName)
                    .ThenBy(t => t.LastName)
                    .Skip(pagination.CalculateSkip())
                    .Take(pagination.PageSize)
                    .ToListAsync();

            return PaginatedResult<LabTechnician>
                .Create(
                    items,
                    totalCount,
                    pagination);
        }

        public async Task<PaginatedResult<LabTechnician>>
    GetAvailableForNewLaboratoryAsync(
        PaginationParams pagination,
        string? searchTerm = null)
        {
            var query = _context.LabTechnicians
                .Where(t =>
                    !t.IsDeleted &&
                    !_context.Laboratories.Any(l =>
                        !l.IsDeleted &&
                        l.HeadTechnicianId == t.Id))
                .Include(t => t.Laboratory)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();

                query = query.Where(t =>
                    (t.FirstName + " " + t.LastName)
                        .ToLower()
                        .Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(t => t.FirstName)
                .ThenBy(t => t.LastName)
                .Skip(pagination.CalculateSkip())
                .Take(pagination.PageSize)
                .ToListAsync();

            return PaginatedResult<LabTechnician>
                .Create(items, totalCount, pagination);
        }


        public async Task<bool> IsHeadTechnicianAsync(int technicianId)
        {
            return await _context.Laboratories
                .AnyAsync(l =>
                    !l.IsDeleted &&
                    l.HeadTechnicianId == technicianId);
        }

    }
}