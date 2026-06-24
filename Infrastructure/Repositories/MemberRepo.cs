using Domain.Identity;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MemberRepo : IMemberRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public MemberRepo(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser?> IsValidUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser?> IsValidEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> IsValidPasswordAsync(string password, ApplicationUser user)
        {
            if (await _userManager.CheckPasswordAsync(user, password))
                return user;
            return null;
        }

        public async Task<IdentityResult> RegisterAsync(ApplicationUser applicationUser, string password)
        {
            return await _userManager.CreateAsync(applicationUser, password);
        }

        public async Task AddRoleAsync(ApplicationUser user, string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<string?> GetRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetPermissionsAsync(string roleName)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
                return Enumerable.Empty<string>();

            var perms = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.Permission.ToString())
                .ToListAsync();

            return perms;
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}
