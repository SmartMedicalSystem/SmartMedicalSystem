using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.User
{
    public class UserUpdateDto
    {
        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
