using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Identity;

namespace Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task AddRoleAsync(ApplicationUser user, IEnumerable<string> roleNames);
    }
}
