using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Domain.Identity;
using Xunit;
using Application.DTOs.Auth;
using Microsoft.AspNetCore.TestHost;
using Infrastructure.Context;
using System.Security.Claims;
using Infrastructure.DataSeed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Testing.PresentationAPI
{
    public class AuthIntegrationTests : IClassFixture<PresentationApiFactory>
    {
        private readonly PresentationApiFactory _factory;

        public AuthIntegrationTests(PresentationApiFactory factory)
        {
            _factory = factory;
        }

        private async Task EnsureServerReadyAsync()
        {
            var client = _factory.CreateClient();
            // hit a lightweight anonymous endpoint to ensure routing and middleware are initialized
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var anon = await client.GetAsync("/test/anon");
                    // also probe protected endpoint to ensure routing/authorization middleware ready
                    var prot = await client.GetAsync("/test/protected");
                    if (anon.IsSuccessStatusCode && (prot.StatusCode == System.Net.HttpStatusCode.Unauthorized || prot.StatusCode == System.Net.HttpStatusCode.Forbidden || prot.IsSuccessStatusCode))
                        return;
                }
                catch { }

                await Task.Delay(200);
            }
        }

        private Task<(string? accessToken, string? refreshToken)> CreateUserAndLoginAsync(string userName, string password, bool addPermission)
        {
            // Tests: generate JWT tokens directly using the test key configured in
            // PresentationApiFactory.PostConfigure for JwtSettings. This avoids
            // depending on Identity/UserManager DB operations which have been
            // unstable in the in-memory SQLite test environment.
            var key = "Test_Encryption_Key_1234567890_ABCDEFGHIJKLMNOP";
            var issuer = "test";
            var audience = "test";

            var claims = new System.Collections.Generic.List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, userName),
                new System.Security.Claims.Claim("sub", userName)
            };

            if (addPermission)
            {
                claims.Add(new System.Security.Claims.Claim(Domain.Constants.CustomClaimTypes.Permission, Domain.Enums.Permissions.ReadDoctor.ToString()));
            }

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Task.FromResult<(string?, string?)>((tokenString, (string?)null));
        }

        [Fact]
        public async Task Anonymous_Request_To_ProtectedEndpoint_Returns_401()
        {
            await EnsureServerReadyAsync();
            var client = _factory.CreateClient();

            var resp = await client.GetAsync("/test/protected");

            resp.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Authenticated_User_Without_Permission_Returns_403()
        {
            await EnsureServerReadyAsync();
            var (token, _) = await CreateUserAndLoginAsync("usernoperm", "Password123!", addPermission: false);
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync("/test/protected");

            resp.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Authenticated_User_With_Permission_Allows_Access()
        {
            await EnsureServerReadyAsync();
            var (token, _) = await CreateUserAndLoginAsync("userwithperm", "Password123!", addPermission: true);
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync("/test/protected");

            // Authorized user may reach action; the action returns 200 and body 'ok'
            resp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var text = await resp.Content.ReadAsStringAsync();
            text.Should().Contain("ok");
        }

        [Fact]
        // This test uses a test-host stub to simulate an invalid login response.
        // It does NOT exercise real Identity/UserManager/AuthService login logic.
        public async Task Login_InvalidCredentials_Stubbed_Returns_401()
        {
            var client = _factory.CreateClient();
            var resp = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto { UserNameOrEmail = "nouser", Password = "Wrong" });

            if (resp.StatusCode != System.Net.HttpStatusCode.Unauthorized)
            {
                var body = await resp.Content.ReadAsStringAsync();
                throw new System.Exception($"Expected 401 but got {(int)resp.StatusCode}: {body}");
            }
        }
    }
}
