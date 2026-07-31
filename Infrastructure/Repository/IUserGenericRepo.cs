using Domain.Identity;

namespace Domain.IRepository
{
    public interface IUserGenericRepo
    {
        Task<ApplicationUser?> GetByUserIdAsync(string userId);
    }
}