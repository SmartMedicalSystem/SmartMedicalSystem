using Application.Services.Abstraction;
using Domain.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                return int.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }

        public int? BasePersonId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirstValue(CustomClaimTypes.BasePersonId);

                return int.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }
    }
}
