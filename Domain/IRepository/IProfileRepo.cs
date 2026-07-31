using Domain.Entities;
using Domain.Entities.Person;
using Domain.Identity;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IProfileRepo : IGenericRepository<BasePerson> 
    {
        /// <summary>Fetches the profile belonging to the currently logged-in technician (self-service page).</summary>

 
        /// <summary>Used on Create to make sure a technician doesn't get two profiles.</summary>

    }
}