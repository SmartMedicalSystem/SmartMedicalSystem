using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string RefreshToken { get; set; } = string.Empty;

        public string? PersonName { get; set; }
    }
}