using Domain.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.StartupExtensions
{
    public static class ConfigureServiceExtension
    {
        public static void ConfigureService(this IServiceCollection services   , IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            
            services.AddSwaggerGen();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));



            // Configure Identity with custom user and role classes


            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, int>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, int>>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            });


            // Configure Athoization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("Admin"));
                options.AddPolicy("PatientPolicy", policy =>
                    policy.RequireRole("Patient"));
                options.AddPolicy("DepartmentManagerPolicy", policy =>
                    policy.RequireRole("DepartmentManager"));
                options.AddPolicy("DoctorPolicy", policy =>
                    policy.RequireRole("Doctor"));
                options.AddPolicy("LabTechnicianPolicy", policy =>
                    policy.RequireRole("LabTechnician"));

            });


        }
    }
}
