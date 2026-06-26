using Application.Interfaces.Attributes;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class PermessionHandler(ApplicationDbContext dbContext  , CheckPermissionAttributeAbstract checkPermissionAttribute)
    {
        
       
      
        public async Task<bool> CheckPermission( ClaimsIdentity claimsIdentity)
        {
            var RoleID =int.Parse( claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var HasPermissoin = dbContext.Set<RolePermission>().Any(
                x => x.RoleId == RoleID    && 
                x.PermissionId.ToString() ==checkPermissionAttribute.Permission.ToString()
            );
            return HasPermissoin;
        }


    }
  
}
