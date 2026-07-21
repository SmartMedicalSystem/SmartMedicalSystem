using System.Security.Cryptography;
using Domain.Entities;
using Domain.Entities.Person;
using Domain.Identity;
using Domain.IRepository;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    /// <summary>
    /// Identity-backed implementation of IMemberRepo. Wraps UserManager/RoleManager
    /// for account operations and reads permissions straight from the
    /// RolePermissions/Permissions tables via the ApplicationDbContext.
    /// </summary>
    /// 

    public class MemberRepository : IMemberRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public MemberRepository(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser?> FindByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _userManager.FindByNameAsync(usernameOrEmail)
                   ?? await _userManager.FindByEmailAsync(usernameOrEmail);
        }

        public async Task<ApplicationUser?> IsValidUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser?> IsValidEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> IsValidPasswordAsync(
            string password,
            ApplicationUser user)
        {
            return await _userManager.CheckPasswordAsync(user, password)
                ? user
                : null;
        }

        public async Task<IdentityResult> RegisterAsync( ApplicationUser applicationUser,
            string password)
        {
            return await _userManager.CreateAsync(applicationUser, password);
        }

        public async Task<bool> AddRoleAsync( ApplicationUser user,string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new Exception($"Role '{roleName}' does not exist.");

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }

        public async Task<string?> GetRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return roles.FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetPermissionsAsync(string roleName)
        {
            // Prefer role claims (AspNetRoleClaims). This is the new source of truth.
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
                return Enumerable.Empty<string>();

            var roleClaims = await _roleManager.GetClaimsAsync(role);

            var permissionsFromClaims = roleClaims
                .Where(c => c.Type == Domain.Constants.CustomClaimTypes.Permission)
                .Select(c => c.Value)
                .Distinct()
                .ToList();

            return permissionsFromClaims;
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }


        public async Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;
           
            return await _userManager.GeneratePasswordResetTokenAsync(user);

        }



        public async Task<IdentityResult> ResetPasswordAsync(
           ApplicationUser user,
           string newPassword)
        {
            var resetToken =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            return await _userManager.ResetPasswordAsync(
                user,
                resetToken,
                newPassword);
        }


        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId , BasePerson person)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id== person.Id);
        }
    }
}
