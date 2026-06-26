using Domain.Identity;
using Domain.Repositories;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

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

    public async Task<IdentityResult> RegisterAsync(
        ApplicationUser applicationUser,
        string password)
    {
        return await _userManager.CreateAsync(applicationUser, password);
    }

    public async Task AddRoleAsync(
        ApplicationUser user,
        string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            throw new Exception($"Role '{roleName}' does not exist.");

        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<string?> GetRoleAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        return roles.FirstOrDefault();
    }

    public async Task<IEnumerable<string>> GetPermissionsAsync(string roleName)
    {
        var role = await _roleManager.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
            return Enumerable.Empty<string>();

        return await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == role.Id)
            .Select(rp => rp.permission.Name)
            .ToListAsync();
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

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}