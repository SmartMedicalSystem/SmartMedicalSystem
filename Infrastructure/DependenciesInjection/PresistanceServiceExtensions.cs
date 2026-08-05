
using Application.Services;
using Application.Services.Abstraction;
using Application.Services.Abstraction.Auth;
using Application.Services.Auth;
using Domain.Identity;
using Domain.IRepository;
using Infrastructure.Context;
using Infrastructure.Context.Configurations.Jwt;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
//using Infrastructure.Services;
using Infrastructure.Middleware;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
//using Application.Services.Abstraction;
using Application.Interfaces;

namespace Infrastructure.DependenciesInjection
{
    public static class PresistanceServiceExtensions
    {
        public static IServiceCollection AddinfrastructreServices
            (this IServiceCollection services, IConfiguration configuration, Microsoft.Extensions.Hosting.IHostEnvironment? env = null)
        {
            // In normal environments (Development/Staging/Production) register the
            // application's SQL Server DbContext. When running under the dedicated
            // "Testing" environment (used by integration tests) the test host will
            // register its own in-memory SQLite DbContext to avoid provider conflicts.
            // IHostEnvironment has IsEnvironment extension in Microsoft.Extensions.Hosting
            var isTesting = false;
            if (env != null)
            {
                // Use the known IsEnvironment extension method by referencing the extension class name
                // from the Hosting assembly via reflection fallback to avoid requiring an extra using.
                try
                {
                    var mi = typeof(Microsoft.Extensions.Hosting.IHostEnvironment).GetMethod("IsEnvironment", new[] { typeof(string) });
                    if (mi != null)
                    {
                        // This path won't be hit; IsEnvironment is an extension method. Use logical fallback:
                        isTesting = env.EnvironmentName == "Testing";
                    }
                    else
                    {
                        isTesting = env.EnvironmentName == "Testing";
                    }
                }
                catch
                {
                    isTesting = env.EnvironmentName == "Testing";
                }
            }

            if (!isTesting)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    options.UseSqlServer(connectionString);
                });
            }

            // Generic repository (open generic) - covers any BaseEntity that doesn't
            // have a dedicated specialized repository registered below.
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Entity-specific repositories
            services.AddScoped<IDepartmentRepo, DepartmentRepository>();
            services.AddScoped<IDoctorRepo, DoctorRepository>();
            services.AddScoped<ILabTechnicianRepo, LabTechnicianRepository>();
            services.AddScoped<ILabTestRepo, LabTestRepository>();
            services.AddScoped<ILabTestElementRepo, LabTestElementRepository>();
            services.AddScoped<INotificationRepo, NotificationRepository>();
            services.AddScoped<IPatientRepo, PatientRepository>();
            services.AddScoped<IPatientResultRepo, PatientResultRepository>();
            services.AddScoped<IPatientResultElementRepo, PatientResultElementRepository>();
            services.AddScoped<IPatientRagDocumentRepo, PatientRagDocumentRepository>();
            services.AddScoped<IRequestLabsRepo, RequestLabsRepository>();
            services.AddScoped<IRequestLabTestRepo, RequestLabTestRepository>();
            services.AddScoped<ISessionRepo, SessionRepository>();
            services.AddScoped<ITestElementRepo, TestElementRepository>();
            services.AddScoped<IMemberRepo, MemberRepository>();

            // Person generic repository (handles encrypted SSN lookups)
            // Encryption service for SSN handling
            services.AddScoped<Infrastructure.Services.NationalIDEncryptionService>();

            // Register IPersonGenericRepo using the closed generic implementation type
            services.AddScoped(typeof(Domain.IRepository.IPersonGenericRepo), typeof(Infrastructure.Repository.PersonGenericRepo<Domain.Entities.Person.BasePerson>));

            services.AddScoped<IMemberRepo, MemberRepository>();
            services.AddScoped<ILaboratoryRepo, LaboratoryRepository>();
            services.AddScoped<ILaboratoryService, LaboratoryService>();
            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Auth-related services (Application-layer contracts, Infrastructure-layer implementations)
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.AddScoped<ITokenService, TokenService>();

            // ASP.NET Core Identity, using our custom ApplicationUser/ApplicationRole
            // In the Testing environment tests register their own DbContext and
            // Identity registration against SQLite. Skip Identity registration
            // here when running under "Testing" to avoid provider conflicts.
            if (!isTesting)
            {
                services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // Reasonable defaults; tune as needed.
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                })
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
            }

            // JWT bearer authentication
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
                // Audit authentication events
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        try
                        {
                            var audit = context.HttpContext.RequestServices.GetService<IAuditService>();
                            var userId = context.Principal?.FindFirst("sub")?.Value ?? context.Principal?.Identity?.Name;
                            audit?.LogAuthenticationEvent(userId, "TokenValidated", "JWT token validated");
                        }
                        catch { }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        try
                        {
                            var audit = context.HttpContext.RequestServices.GetService<IAuditService>();
                            audit?.LogAuthenticationEvent(null, "AuthenticationFailed", context.Exception?.Message ?? "");
                        }
                        catch { }

                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/notificationHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            // Permission-based authorization: a custom policy provider builds a policy
            // on the fly for every "Permission:{Name}" policy generated by
            // [HasPermission(Permissions.X)] / CheckPermissionAttributeAbstract, backed
            // by PermissionAuthorizationHandler which checks the caller's JWT claims.
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // Logging, auditing and monitoring services
            services.AddLogging();
            services.AddHealthChecks();

            // Audit service records authentication/authorization/rate-limit events
            services.AddScoped<IAuditService, AuditService>();

            // Rate limiting service and memory cache used by middleware
            IServiceCollection serviceCollection = services.AddMemoryCache();
            services.AddSingleton<IRateLimitService, InMemoryRateLimitService>();

            services.AddControllers()
               .AddJsonOptions(options =>
                  {
                       options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                  });
            services.AddAuthorization();


            services.AddScoped<Infrastructure.Middleware.LoggingActionFilter>();

            services.AddControllers(options =>
            {
                // global action filter to log every controller endpoint (resolve from DI)
                options.Filters.AddService<Infrastructure.Middleware.LoggingActionFilter>();
            });
            services.AddScoped<IUserGenericRepo, UserGenericRepo<ApplicationUser>>();


            return services;
        }
    }
}
