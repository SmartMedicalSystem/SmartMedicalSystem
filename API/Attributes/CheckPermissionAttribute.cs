using API.Filters;
using Application.Interfaces.Attributes;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security;

namespace API.Attributes
{
    public class CheckPermissionAttribute : CheckPermissionAttributeAbstract
    {
        public CheckPermissionAttribute(Permissions permissions) : base(permissions)
        {
        }
    }
}
