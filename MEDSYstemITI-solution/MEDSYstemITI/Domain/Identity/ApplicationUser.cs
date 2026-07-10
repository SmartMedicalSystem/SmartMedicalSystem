using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    // ASSUMPTION: this class was not in the entities you provided - it's
    // added because Notification.User needs a concrete Identity user type,
    // and because you asked for role-based accounts (Admin/Doctor/Patient).
    // Optional links to Doctor/Patient let a login account be tied to the
    // matching person record; remove them if that's not how you want it.
    public class ApplicationUser : IdentityUser<int>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? SSN { get; set; }
    }
}
