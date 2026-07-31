using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace Testing.PresentationAPI
{
    // Test hosted service to probe the Authorization policy provider at
    // startup so we can confirm PermissionPolicyProvider.GetPolicyAsync
    // is executed in the test host.
    public class TestPolicyInitializer : IHostedService
    {
        private readonly IServiceProvider _provider;

        public TestPolicyInitializer(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            var policyProvider = scope.ServiceProvider.GetRequiredService<IAuthorizationPolicyProvider>();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
            try
            {
                var policy = await policyProvider.GetPolicyAsync("Permission:ReadDoctor");
                var exists = policy != null;
                var reqCount = policy?.Requirements?.Count() ?? 0;
                System.Console.WriteLine($"[TestDiag] TestPolicyInitializer resolved policy: {(exists ? "OK" : "null")}; Requirements:{reqCount}");

                // Also run an explicit authorization check to exercise the handler
                // using a principal that has the permission claim and one that does not.
                var principalWith = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("sub", "startupuser"),
                    new System.Security.Claims.Claim(Domain.Constants.CustomClaimTypes.Permission, Domain.Enums.Permissions.ReadDoctor.ToString())
                }, "Test"));

                var resWith = await authService.AuthorizeAsync(principalWith, null, "Permission:ReadDoctor");
                System.Console.WriteLine($"[TestDiag] Authorization with permission: {resWith.Succeeded}");

                var principalWithout = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("sub", "startupuser2")
                }, "Test"));

                var resWithout = await authService.AuthorizeAsync(principalWithout, null, "Permission:ReadDoctor");
                System.Console.WriteLine($"[TestDiag] Authorization without permission: {resWithout.Succeeded}");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("[TestDiag] TestPolicyInitializer failed to resolve policy or authorize: " + ex.ToString());
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
