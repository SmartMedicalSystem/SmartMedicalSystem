using Domain.Identity;
using Domain.IRepository;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repository
{
    public class UserGenericRepo<T> : IUserGenericRepo
        where T : ApplicationUser
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserGenericRepo(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByUserIdAsync(
            string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> UpdateUserAsync(
            ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}