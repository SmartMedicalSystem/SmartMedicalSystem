using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.IRepository
{

    public interface IMemberRepo
    {
        Task<ApplicationUser?> FindByUsernameOrEmailAsync(string usernameOrEmail);

        Task<ApplicationUser?> IsValidUsernameAsync(string username);

        Task<ApplicationUser?> IsValidEmailAsync(string email);

        Task<ApplicationUser?> IsValidPasswordAsync(
            string password,
            ApplicationUser user);

        Task<IdentityResult> RegisterAsync(
            ApplicationUser applicationUser,
            string password);

        Task AddRoleAsync(
            ApplicationUser user,
            string roleName);

        Task<string?> GetRoleAsync(  ApplicationUser user);

        Task<IEnumerable<string>> GetPermissionsAsync(
            string roleName);

        Task UpdateAsync(ApplicationUser user);

        Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken);


        Task<string?> GeneratePasswordResetTokenAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);

        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);

    }
}
