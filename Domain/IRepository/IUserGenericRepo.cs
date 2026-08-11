using Domain.Identity;
using Microsoft.AspNetCore.Identity;

public interface IUserGenericRepo
{
    Task<ApplicationUser?> GetByUserIdAsync(string userId);

    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

    Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}