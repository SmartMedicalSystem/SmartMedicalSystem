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
    public class PermessionHandler(ApplicationDbContext dbContext  , CheckPermissionAttributeAbstract checkPermissionAttribute, ApplicationRole role)
    {
        
       
      
        public async Task<bool> CheckPermission( )
        {
            var HasPermissoin = dbContext.Set<RolePermission>().Any(
                x => x.RoleId == role.Id    && 
                x.PermissionId.ToString() ==checkPermissionAttribute.Permission.ToString()
            );
            return HasPermissoin;
            


        }


    }
  
}
