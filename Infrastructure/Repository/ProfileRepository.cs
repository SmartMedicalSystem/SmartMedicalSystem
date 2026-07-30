using Domain.Entities;
using Domain.Entities.Person;
using Domain.Identity;
using Domain.IRepository;
using Domain.Models;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ProfileRepository : GenericRepository<BasePerson>, IProfileRepo
    {

        private readonly ApplicationDbContext _context;

        public ProfileRepository(ApplicationDbContext context) : base(context)
        {
        }

 
        public async Task<BasePerson?> GetByPersonIdAsync(int id)
        {
            return await _context.FindAsync<BasePerson>(id);
            
        }

    
    }
}
