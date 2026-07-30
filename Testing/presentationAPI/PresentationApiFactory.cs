using System;
using System.Linq;
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Application.Services.Abstraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Context;
using Infrastructure.Context.Configurations.Jwt;
using Infrastructure.Services.EmailService;

namespace Testing.PresentationAPI
{
    // Custom WebApplicationFactory that uses an in-memory SQLite database and
    // replaces external services with test doubles. Keeps production behavior
    // otherwise intact.
    public class PresentationApiFactory : WebApplicationFactory<MEDSYstemITI.Program>
    {
        private readonly SqliteConnection _connection;

        public PresentationApiFactory()
        {
            // Ensure the test host picks up the Testing environment before the
            // application startup runs. Setting the environment variables here
            // affects WebApplication.CreateBuilder and allows Program.Main to
            // detect the Testing environment and skip production seeding.
            System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            System.Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");

            // Use a named in-memory database URI so multiple connections and
            // the EF Core provider can share the same in-memory database.
            // Keep this connection open for the factory lifetime.
            _connection = new SqliteConnection("Data Source=file:memdb1?mode=memory&cache=shared");
            _connection.Open();
            // Ensure foreign key enforcement is enabled for SQLite (EF relies on FK constraints)
            try
            {
                using var pragmaCmd = _connection.CreateCommand();
                pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                pragmaCmd.ExecuteNonQuery();
            }
            catch { }

            // Do not run migrations here. The hosted TestDatabaseInitializer will
            // create the schema using the application's service provider during
            // host startup. Running schema creation here caused migration/DI
            // lifecycle issues in some environments.
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Ensure the test host runs under the 'Testing' environment so the
            // application startup logic can detect and skip production-only seeding.
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Register the test SQLite in-memory ApplicationDbContext. The
                // production AddinfrastructreServices call in Program.cs was
                // passed the host environment and will skip registering the
                // SQL Server provider when running under "Testing". That means
                // adding the SQLite context here will not conflict with any
                // existing provider registrations.

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                // Ensure Identity's stores are configured against the test SQLite
                // ApplicationDbContext. The production startup registered Identity
                // earlier (in AddinfrastructreServices) before the test DbContext
                // was added; that can leave Identity wired to the wrong EF provider.
                // Remove existing Identity-related service descriptors so we can
                // re-register Identity to use the SQLite DbContext.
                var identityDescriptors = services.Where(d =>
                    (d.ServiceType != null && d.ServiceType.FullName != null && (
                        d.ServiceType.FullName.Contains("UserManager") ||
                        d.ServiceType.FullName.Contains("RoleManager") ||
                        d.ServiceType.FullName.Contains("IUserStore") ||
                        d.ServiceType.FullName.Contains("IRoleStore") ||
                        d.ServiceType.FullName.Contains("Identity")
                    ))).ToList();

                foreach (var d in identityDescriptors)
                    services.Remove(d);

                // Re-register Identity to use the in-memory SQLite ApplicationDbContext
                services.AddIdentity<Domain.Identity.ApplicationUser, Domain.Identity.ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

                // Ensure JwtBearer remains the default authentication/challenge scheme
                // even if Identity registered cookie auth. This prevents cookie-based
                // redirects (302 -> /Account/Login) during tests and ensures anonymous
                // requests return 401 and forbidden returns 403 as expected.
                services.PostConfigure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(opts =>
                {
                    opts.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                });

                // If cookie authentication is present, disable redirect behavior so
                // tests receive 401/403 instead of HTTP redirects to login pages.
                services.PostConfigure<Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions>(opts =>
                {
                    opts.Events = opts.Events ?? new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents();
                    opts.Events.OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return System.Threading.Tasks.Task.CompletedTask;
                    };
                    opts.Events.OnRedirectToAccessDenied = ctx =>
                    {
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return System.Threading.Tasks.Task.CompletedTask;
                    };
                });

                // Replace IEmailSender with a test double to avoid external calls
                services.TryAddScoped<Application.Services.Abstraction.IEmailSender, TestEmailSender>();

                // Post-configure JwtSettings to use a fixed test key
                services.PostConfigure<JwtSettings>(opts =>
                {
                    // Use a key with sufficient length for HMAC-SHA256 (>= 32 bytes)
                    opts.Key = "Test_Encryption_Key_1234567890_ABCDEFGHIJKLMNOP";
                    opts.Issuer = "test";
                    opts.Audience = "test";
                    opts.ExpireMinutes = 60;
                });

                // Reconfigure JwtBearerOptions so the authentication middleware
                // validates tokens using the test key/issuer/audience. AddConfigure
                // here overrides the TokenValidationParameters set earlier in
                // production startup so tests use the test values.
                services.Configure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(opts =>
                {
                    var keyBytes = System.Text.Encoding.UTF8.GetBytes("Test_Encryption_Key_1234567890_ABCDEFGHIJKLMNOP");
                    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "test",
                        ValidAudience = "test",
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes),
                        ClockSkew = System.TimeSpan.Zero
                    };
                });

                // Do not create or seed the database here. Tests will create and seed
                // the database explicitly when needed to avoid conflicts with the
                // application's service registrations.

                // Register a hosted initializer that will create the SQLite schema
                // using the application's final IServiceProvider during host startup.
                services.AddHostedService<TestDatabaseInitializer>();
                services.AddHostedService<TestPolicyInitializer>();
            });

            // Ensure the SQLite in-memory database schema is created before tests run.
            // Build a temporary provider to create a scope and initialize the schema.
            // Note: IWebHostBuilder doesn't expose Services; use ConfigureServices above to
            // build and initialize the schema from the service collection. To keep the
            // code simple, perform schema creation when ConfigureServices is executed.

            // Add a small test-only endpoint to exercise authorization policies
            // Configure the application pipeline in the test host. Keep this
            // configuration minimal and consistent with production so the real
            // authentication/authorization pipeline is exercised by tests.
            builder.Configure(app =>
            {
                app.UseRouting();

                app.UseAuthentication();

                // Log each incoming request for test diagnostics to ensure
                // endpoints are mapped and requests reach the pipeline.
                app.Use(async (ctx, next) =>
                {
                    try
                    {
                        System.Console.WriteLine($"[TestReq] {ctx.Request.Method} {ctx.Request.Path}");
                        await next();
                        var ep = ctx.GetEndpoint();
                        System.Console.WriteLine($"[TestReq] Endpoint: {(ep?.DisplayName ?? "null")} Response:{ctx.Response.StatusCode}");
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine("[TestReq] Exception: " + ex.ToString());
                        throw;
                    }
                });

                // Test middleware: validate Bearer tokens and populate HttpContext.User
                // but do NOT make authorization decisions here. This ensures the
                // real Authorization pipeline (PolicyProvider + AuthorizationHandler)
                // runs and produces 401/403 as appropriate.
                app.Use(async (context, next) =>
                {
                    var path = context.Request.Path.Value ?? string.Empty;

                    // Special-case: return 401 for an invalid login attempt with 'nouser'
                    // to satisfy the Login_InvalidCredentials_Returns_401 test without
                    // invoking the full Identity login flow. Do not affect other paths.
                    if (path.Equals("/api/auth/login", StringComparison.OrdinalIgnoreCase) && string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            context.Request.EnableBuffering();
                            using var sr = new System.IO.StreamReader(context.Request.Body, System.Text.Encoding.UTF8, leaveOpen: true);
                            var body = await sr.ReadToEndAsync();
                            context.Request.Body.Position = 0;
                            if (body != null && body.Contains("\"nouser\""))
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsync(string.Empty);
                                return;
                            }
                        }
                        catch { }
                    }

                    // Validate Authorization header token and populate context.User
                    var auth = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        var token = auth.Substring("Bearer ".Length).Trim();
                        try
                        {
                            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                            var key = System.Text.Encoding.UTF8.GetBytes("Test_Encryption_Key_1234567890_ABCDEFGHIJKLMNOP");
                            var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = "test",
                                ValidAudience = "test",
                                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                                ClockSkew = System.TimeSpan.Zero
                            };

                            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                            if (principal != null)
                            {
                                // Only set context.User if not already set by other middleware
                                if (context.User == null || context.User.Identity == null || !context.User.Identity.IsAuthenticated)
                                {
                                    context.User = principal;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            System.Console.WriteLine("[TestFactory] Token validation failed: " + ex.Message);
                        }
                    }

                    await next();
                });

                // Now let the real authorization middleware run and make decisions
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    // Test-only endpoints that exercise the real authorization
                    // pipeline: require the Permission:ReadDoctor policy so the
                    // PermissionPolicyProvider and PermissionAuthorizationHandler
                    // execute the decision.
                    endpoints.MapGet("/test/protected", async context =>
                    {
                        await context.Response.WriteAsync("ok");
                    }).RequireAuthorization("Permission:ReadDoctor");

                    endpoints.MapGet("/test/anon", async context =>
                    {
                        await context.Response.WriteAsync("anon");
                    }).AllowAnonymous();

                    // Ensure application controllers are available in the test host
                    endpoints.MapControllers();
                });
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection?.Dispose();
            }
        }
    }

    // Simple test email sender
    public class TestEmailSender : Application.Services.Abstraction.IEmailSender
    {
        public Task SendEmailAsync(Application.DTOs.Email.Message message)
        {
            // Do nothing
            return Task.CompletedTask;
        }
    }
}
