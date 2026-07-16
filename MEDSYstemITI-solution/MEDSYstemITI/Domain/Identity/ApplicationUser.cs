using System;
using Domain.Entities;
using Domain.Entities.Baseperson;
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
        public string FullName { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        // Optional navigation to the domain person record that represents this account
        public BasePerson? Person { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }= new List<ApplicationUserRole>();

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }

        public bool AllowLogin { get; set; }

        public bool AccountActive { get; set; }

        public bool ReceiveNotifications { get; set; }
    }
}
