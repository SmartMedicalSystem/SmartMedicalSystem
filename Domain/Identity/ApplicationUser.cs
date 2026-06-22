using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {

        public IList<ApplicationRole> Roles { get; set; }
    }
}
