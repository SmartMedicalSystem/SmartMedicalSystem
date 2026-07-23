using Domain.Entities;
using Domain.Identity;
using Domain.IRepository;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class LabTechProfileRepository : GenericRepository<LabTechnician>, ILabTechProfileRepo
    {

        private readonly ApplicationDbContext _context;
        public LabTechProfileRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<LabTechnician?> GetByPersonIdAsync(int id)
        {
            return await _context.FindAsync<LabTechnician>(id);
            
        }

      


    
    }
}
