using Domain.Identity;
using Domain.IRepository;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repository
{
    public class UserGenericRepo<T> : IUserGenericRepo where T : ApplicationUser
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserGenericRepo(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<ApplicationUser?> GetByUserIdAsync(
            string personId)
        {
            var userId = _userManager.Users
                .Where(u => u.PersonId.ToString() == personId)
                .Select(u => u.Id)
                .FirstOrDefault().ToString();
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> UpdateUserAsync(
            ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}