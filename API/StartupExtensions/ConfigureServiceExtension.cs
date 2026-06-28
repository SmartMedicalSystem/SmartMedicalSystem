using Antlr.Runtime;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Identity;
using Domain.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Context;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.StartupExtensions
{
    public static class ConfigureServiceExtension
    {
        public static void ConfigureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();


            // Configure DbContext with SQL Server and logging
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging());

            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            // Configure Identity with custom user and role classes


            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;

                options.Password.RequireDigit = true;

                options.Password.RequireUppercase = true;

                options.Password.RequireLowercase = true;

                options.Password.RequireNonAlphanumeric = true;

                options.User.RequireUniqueEmail = true;

                options.Lockout.MaxFailedAccessAttempts = 5;

                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);

                options.Lockout.AllowedForNewUsers = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();



            //authentication and jwt 
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;

                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,

                            ValidateAudience = true,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,

                            ValidIssuer =
                                configuration["Jwt:Issuer"],

                            ValidAudience =
                                configuration["Jwt:Audience"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        configuration["Jwt:Key"]
                                        ?? throw new InvalidOperationException(
                                            "JWT Key Missing"))),

                            ClockSkew = TimeSpan.Zero
                        };
                });
         


            // Configure Athoization
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "AdminPolicy",
                    policy => policy.RequireRole("Admin"));

                options.AddPolicy(
                    "DoctorPolicy",
                    policy => policy.RequireRole("Doctor"));

                options.AddPolicy(
                    "DepartmentManagerPolicy",
                    policy => policy.RequireRole("DepartmentManager"));

                options.AddPolicy(
                    "LabTechnicianPolicy",
                    policy => policy.RequireRole("LabTechnician"));
            });

            // Configure CORS 
            //this is developnent url 

            services.AddCors(options =>
            {
                options.AddPolicy("SmartMedicalSystemCorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:5000")
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });


            services.AddScoped<ITokenService, TokenService>();

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));


        }
    }
}


