using Domain.Enums;
using Application.Services.Auth;

namespace API.Attributes
{
    public class CheckPermissionAttribute : CheckPermissionAttributeAbstract
    {
        public CheckPermissionAttribute(Permissions permissions) : base(permissions)
        {
            // Policy is constructed in base class using PolicyPrefix + permission
        }
    }
}
