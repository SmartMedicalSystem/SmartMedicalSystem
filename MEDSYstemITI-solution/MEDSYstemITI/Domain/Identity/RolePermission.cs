using Domain.Enums;

namespace Domain.Identity
{
    public class RolePermission
    {
        public int PermissionId { get; set; }

        public int RoleId { get; set; }

        public ApplicationRole Role { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
    }
}