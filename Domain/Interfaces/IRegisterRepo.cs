using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IRegisterRepo
    {
        public Task RegisterAsync(IdentityUser user, string password);
        public Task UnregisterAsync(string email);

        public Task<bool> IsEmailExistsAsync(string email);
    }
}
