using Domain.Entities;
using Domain.Entities.Person;
using Domain.IRepository;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class ProfileRepository : GenericRepository<BasePerson>, IProfileRepo
    {
        private readonly ApplicationDbContext _context;

        public ProfileRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<BasePerson?> GetByIdAsync(int id)
        {
            // If the person is a Lab Technician, load the Laboratory navigation property.
            var labTech = await _context.Set<LabTechnician>()
                .Include(lt => lt.Laboratory)
                .FirstOrDefaultAsync(lt => lt.Id == id && !lt.IsDeleted);

            if (labTech != null)
                return labTech;

            // Otherwise return the person normally.
            return await _context.Set<BasePerson>()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<BasePerson?> GetByPersonIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }
    }
}