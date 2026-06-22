using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Identity
{
    public  class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole applicationRole { get; set; }
    }
}
