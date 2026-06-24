using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Repositories
{
    public interface IMemberRepo
    {

       

        /// <summary>
        /// 1-register the user
        /// 
        /// 2-validations : check username , email password
        /// </summary>
        /// 

        public  Task<IdentityResult> RegisterAsync(ApplicationUser applicationUser);

        public Task<ApplicationUser> IsValidEmailAsync(string email);

        public Task<ApplicationUser> IsValidPasswordAsync( string password , ApplicationUser applicationUser);


        public Task<ApplicationUser> IsValidUsernameAsync( string username );

        public string GenerateRefreshToken();

        //public Task AddRoleAsync(ApplicationUser user, IEnumerable<string> roleNames);
        //public Task<ApplicationUser> GetRoleAsync(string roleName, ApplicationUser applicationUser);
        
    }
}