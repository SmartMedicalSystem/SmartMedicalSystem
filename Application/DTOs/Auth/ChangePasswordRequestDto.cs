using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class ChangePasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}
