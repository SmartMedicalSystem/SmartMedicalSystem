using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Application.Services.Auth
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]

    public class CheckPermissionAttributeAbstract : AuthorizeAttribute
    {
        public const string PolicyPrefix = "Permission:";

        public CheckPermissionAttributeAbstract(Permissions permissions)
        {
            this.Permission = permissions;
            Policy = $"{PolicyPrefix}{permissions}";
        }

        public Permissions Permission
        {
            get;

        }
    }

    /// <summary>
    /// Friendlier name for CheckPermissionAttributeAbstract, e.g.
    /// [HasPermission(Permissions.CreateDoctor)] on a controller action.
    /// </summary>
    public sealed class HasPermissionAttribute : CheckPermissionAttributeAbstract
    {
        public HasPermissionAttribute(Permissions permission) : base(permission)
        {
        }
    }
}
