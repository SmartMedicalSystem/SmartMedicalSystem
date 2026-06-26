using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]

    public class CheckPermissionAttributeAbstract : AuthorizeAttribute
    {

        public CheckPermissionAttributeAbstract(Permissions permissions)
        {
            this.Permission = permissions;
        }

        public Permissions Permission
        {
            get;

        }
    }
}
