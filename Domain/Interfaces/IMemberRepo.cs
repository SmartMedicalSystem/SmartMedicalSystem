using Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Domain.Repositories;

public interface IMemberRepo
{
    Task<ApplicationUser?> IsValidUsernameAsync(string username);

Task<ApplicationUser?> IsValidEmailAsync(string email);

    Task<ApplicationUser?> IsValidPasswordAsync( string password,ApplicationUser user);

    Task<IdentityResult> RegisterAsync(ApplicationUser applicationUser,string password);

    Task AddRoleAsync(ApplicationUser user, string roleName, IEnumerable<RolePermission> permissions);
    Task AddPermessionAsync(ApplicationRole applicationRole, IEnumerable<RolePermission> rolePermissions);
    Task<string?> GetRoleAsync(  ApplicationUser user);

    Task<IEnumerable<string>> GetPermissionsAsync(
        string roleName);
        
    string GenerateRefreshToken();

}
