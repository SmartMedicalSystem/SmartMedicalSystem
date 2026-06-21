using Domain.Identity;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class RegisterRep : IRegisterRepo
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterRep(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public Task RegisterAsync(IdentityUser user, string password)
        {
            // Implement the logic to register a user using ASP.NET Identity
            return _userManager.CreateAsync(user, password);
        }


        public Task UnregisterAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityUser> GetByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<bool> IsEmailExistsAsync(string email)
        {
            return _userManager.FindByEmailAsync(email).ContinueWith(t => t.Result != null);
        }
    }
}
