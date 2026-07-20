using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class LabTechProfileRepository : GenericRepository<LabTechProfile>, ILabTechProfileRepo
    {
        public LabTechProfileRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<LabTechProfile?> GetByUserIdAsync(int userId)
        {
            return await _context.LabTechProfiles
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsForUserAsync(int userId)
        {
            return await _context.LabTechProfiles
                .AnyAsync(p => p.UserId == userId && !p.IsDeleted);
        }

        public async Task<bool> EmployeeIdExistsAsync(string employeeId)
        {
            return await _context.LabTechProfiles
                .AnyAsync(p => p.EmployeeId == employeeId && !p.IsDeleted);
        }
    }
}
