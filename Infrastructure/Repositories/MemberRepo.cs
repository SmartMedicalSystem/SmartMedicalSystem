using Azure.Core;
using Domain.Identity;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Repositories
{
    public class MemberRepo : IMemberRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public MemberRepo(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> IsValidPasswordAsync(string password , ApplicationUser user)
        {
            if (await _userManager.CheckPasswordAsync(user, password))
                return user;
            else
                return null;
            
        }

   

        public async Task<ApplicationUser> IsValidUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<IdentityResult>  registerAsync(ApplicationUser applicationUser)
        {
           
            
            var result= await _userManager.CreateAsync(applicationUser);
            return result;
        }

        public async Task<ApplicationUser> IsValidEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }





        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public Task<IdentityResult> RegisterAsync(ApplicationUser applicationUser)
        {
            throw new NotImplementedException();
        }

        //public async Task AddRoleAsync(ApplicationUser user, IEnumerable<string> roleNames)
        //{
        //    if (roleNames == null)
        //        return;

        //    foreach (var roleName in roleNames)
        //    {
        //        if (string.IsNullOrWhiteSpace(roleName))
        //            continue;

        //        if (!await _roleManager.RoleExistsAsync(roleName))
        //        {
        //            await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        //        }
        //    }

        //    await _userManager.AddToRolesAsync(user, roleNames);
        //}

        //public Task<IdentityResult> RegisterAsync(ApplicationUser applicationUser)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<ApplicationUser> GetRoleAsync(string roleName, ApplicationUser applicationUser)
        //{
        //    return _roleManager.GetRoleNameAsync();
        //}
    }
}
