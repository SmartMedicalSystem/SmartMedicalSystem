using Domain.Entities.Baseperson;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public BasePerson personRole { get; set; }


        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}