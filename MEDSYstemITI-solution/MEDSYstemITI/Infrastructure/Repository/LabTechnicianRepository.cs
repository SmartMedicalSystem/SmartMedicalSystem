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
                query = query.Where(x => x.Laboratory.Contains(laboratory));
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
    }
}