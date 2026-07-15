using Domain.Entities.Baseperson;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationUser User { get; set; } = null!;


        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}