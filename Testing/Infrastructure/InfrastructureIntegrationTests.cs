using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Context;
using Infrastructure.DataSeed;
using Infrastructure.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Domain.Identity;
using Xunit;

namespace Testing.Infrastructure
{
    public class InfrastructureIntegrationTests
    {
        private ServiceProvider BuildServices(SqliteConnection connection)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connection));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // register IMemberRepo for tests that resolve it from DI
            services.AddScoped<Domain.IRepository.IMemberRepo, global::Infrastructure.Repository.MemberRepository>();

            return services.BuildServiceProvider();
        }

        [Fact]
        public async Task Seeder_RolesAndRoleClaims_AreSeeded_Idempotent()
        {
            // Arrange - create shared in-memory sqlite connection
            using var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var provider = BuildServices(connection);


            using (var scope = provider.CreateScope())
            {
                var scoped = scope.ServiceProvider;
                var context = scoped.GetRequiredService<ApplicationDbContext>();

                // Ensure database created
                context.Database.EnsureCreated();

                var roleManager = scoped.GetRequiredService<RoleManager<ApplicationRole>>();

                // Act: run seeders twice
                await RoleSeeder.SeedAsync(roleManager);
                await RoleClaimsSeeder.SeedAsync(context, roleManager);

                // Capture counts
                var roleCount = await context.Roles.CountAsync();
                var claimCount = await context.RoleClaims.CountAsync();

                // Run again to ensure idempotency
                await RoleSeeder.SeedAsync(roleManager);
                await RoleClaimsSeeder.SeedAsync(context, roleManager);

                var roleCount2 = await context.Roles.CountAsync();
                var claimCount2 = await context.RoleClaims.CountAsync();

                // Assert
                roleCount.Should().BeGreaterThan(0);
                claimCount.Should().BeGreaterThan(0);

                roleCount2.Should().Be(roleCount);
                claimCount2.Should().Be(claimCount);

                // Verify claim type uses CustomClaimTypes.Permission
                var sampleClaim = await context.RoleClaims.FirstOrDefaultAsync();
                sampleClaim.Should().NotBeNull();
                sampleClaim.ClaimType.Should().Be(Domain.Constants.CustomClaimTypes.Permission);
            }
        }

        [Fact]
        public async Task GenericRepository_AddAndQuery_Department_Works()
        {
            using var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var provider = BuildServices(connection);
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();


            using (var scope = provider.CreateScope())
            {
                var scoped = scope.ServiceProvider;
                var context = scoped.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                // Use UnitOfWork from Infrastructure
                var dataProtection = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create("tests");
                var uow = new UnitOfWork(context, dataProtection, userManager);

                var dept = new Domain.Entities.Department("Cardiology","Dr X", 2);
                var added = await uow.Departments.AddAsync(dept);

                added.Id.Should().BeGreaterThan(0);

                var fetched = await uow.Departments.GetByIdAsync(added.Id);
                fetched.Should().NotBeNull();
                fetched!.Name.Should().Be("Cardiology");

                // Soft delete
                var soft = await uow.Departments.SoftDeleteAsync(added.Id);
                soft.Should().BeTrue();

                var exists = await uow.Departments.ExistsAsync(added.Id);
                exists.Should().BeFalse();
            }
        }

        [Fact]
        public async Task MemberRepository_GetPermissionsAsync_Returns_AssignedPermissions_SQLite()
        {
            using var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var provider = BuildServices(connection);

            using (var scope = provider.CreateScope())
            {
                var scoped = scope.ServiceProvider;
                var context = scoped.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                var roleManager = scoped.GetRequiredService<RoleManager<ApplicationRole>>();
                var userManager = scoped.GetRequiredService<UserManager<ApplicationUser>>();
                var memberRepo = scoped.GetRequiredService<Domain.IRepository.IMemberRepo>();

                // create role and add permission claim
                var role = new ApplicationRole { Name = "SqliteRole" };
                await roleManager.CreateAsync(role);
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(Domain.Constants.CustomClaimTypes.Permission, "Sqlite.Permission.A"));

                var permissions = await memberRepo.GetPermissionsAsync("SqliteRole");
                permissions.Should().Contain("Sqlite.Permission.A");
            }
        }
    }
}
