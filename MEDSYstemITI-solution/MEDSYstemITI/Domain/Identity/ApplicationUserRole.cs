using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Identity
{
    public class ApplicationUserRole : IdentityUserRole<int>
    {
        public ApplicationUser User { get; set; } = null!;

        public ApplicationRole Role { get; set; } = null!;
    }
}
