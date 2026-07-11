using Domain.Entities;
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

        public async Task<LabTechnician?> GetByEmployeeIdAsync(string employeeId)
        {
            return await _context.LabTechnicians
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.EmployeeId == employeeId);
        }

        public async Task<LabTechnician?> GetByNationalIdAsync(string nationalId)
        {
            return await _context.LabTechnicians
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.NationalId == nationalId);
        }

        public override async Task<PaginatedResult<LabTechnician>> GetAllPaginatedAsync(PaginationParams pagination)
        {
            var query = _context.LabTechnicians
                .Where(x => !x.IsDeleted);

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