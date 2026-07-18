using System.Reflection;
using Domain.Entities;
using Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Domain.Entities.Baseperson;

namespace Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        int,
        IdentityUserClaim<int>,
        ApplicationUserRole,
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<BasePerson> BasePersons { get; set; }


        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<LabTechnician> LabTechnicians => Set<LabTechnician>();
        public DbSet<LabTest> LabTests => Set<LabTest>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<PatientResult> PatientResults => Set<PatientResult>();
        public DbSet<PatientResultElement> PatientResultElements => Set<PatientResultElement>();
        public DbSet<RequestLabs> RequestLabs => Set<RequestLabs>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<TestElement> TestElements => Set<TestElement>();
        public DbSet<LabTestElement> LabTestElements => Set<LabTestElement>();

        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Must run first: sets up the Identity (AspNetUsers, AspNetRoles, ...) tables/keys.
            base.OnModelCreating(builder);

            // Picks up every IEntityTypeConfiguration<T> in this assembly (the Configurations folder).
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
