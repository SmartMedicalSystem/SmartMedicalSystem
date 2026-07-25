using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.User
{
    public class ChangePasswordRequestDto
    {
        public string CurrentPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}
