using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Context;
using Infrastructure.DataSeed;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.IRepository;
using Domain.Identity;
using Xunit;

namespace Testing.Infrastructure
{
    public class SqlServerIntegrationTests
    {

        private ServiceProvider BuildServices(string connectionString)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // register repository implementations used directly in tests
            services.AddScoped<IMemberRepo, global::Infrastructure.Repository.MemberRepository>();

            return services.BuildServiceProvider();
        }

        [Fact]
        public async Task SqlServer_Migrations_And_Seeders_Run_And_Permissions_Are_Read()
        {
            var cs = Environment.GetEnvironmentVariable("SQLSERVER_INTEGRATION_CONNECTION");
            if (string.IsNullOrWhiteSpace(cs))
            {
                // External SQL Server not configured; do not run SQL Server-specific tests in default runs.
                // Returning early makes the test a no-op when no connection is provided.
                return;
            }

            var provider = BuildServices(cs);

            using var scope = provider.CreateScope();
            var scoped = scope.ServiceProvider;

            var context = scoped.GetRequiredService<ApplicationDbContext>();

            // Apply migrations (production path)
            await context.Database.MigrateAsync();

            // Validate connection and schema
            var canConnect = await context.Database.CanConnectAsync();
            canConnect.Should().BeTrue();

            // Run seeders
            var roleManager = scoped.GetRequiredService<RoleManager<ApplicationRole>>();

            await RoleSeeder.SeedAsync(roleManager);
            await RoleClaimsSeeder.SeedAsync(context, roleManager);

            var roleCount = await context.Roles.CountAsync();
            var claimCount = await context.RoleClaims.CountAsync();

            roleCount.Should().BeGreaterThan(0);
            claimCount.Should().BeGreaterThan(0);

            // Idempotency
            await RoleSeeder.SeedAsync(roleManager);
            await RoleClaimsSeeder.SeedAsync(context, roleManager);

            var roleCount2 = await context.Roles.CountAsync();
            var claimCount2 = await context.RoleClaims.CountAsync();

            roleCount2.Should().Be(roleCount);
            claimCount2.Should().Be(claimCount);

            // Verify claim type
            var sample = await context.RoleClaims.FirstOrDefaultAsync();
            sample.Should().NotBeNull();
            sample.ClaimType.Should().Be(Domain.Constants.CustomClaimTypes.Permission);

            // MemberRepository.GetPermissionsAsync using a newly created role + claim
            var memberRepo = scoped.GetRequiredService<IMemberRepo>();

            var role = new ApplicationRole { Name = "IntegrationTestRole" };
            await roleManager.CreateAsync(role);
            await roleManager.AddClaimAsync(role, new Claim(Domain.Constants.CustomClaimTypes.Permission, "Test.Permission.X"));

            var permissions = await memberRepo.GetPermissionsAsync("IntegrationTestRole");
            permissions.Should().Contain("Test.Permission.X");
        }
    }
}
