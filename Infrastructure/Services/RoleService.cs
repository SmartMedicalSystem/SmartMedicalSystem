using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.Services;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task AddRoleAsync(ApplicationUser user, IEnumerable<string> roleNames)
        {
            if (roleNames == null)
                return;

            foreach (var roleName in roleNames)
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    continue;

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }

            await _userManager.AddToRolesAsync(user, roleNames);
        }
    }
}
