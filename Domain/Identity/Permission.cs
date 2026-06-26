using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Identity
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<RolePermission> RolePermissions { get; set; }
     = new List<RolePermission>();
    }
}
