using Application.DependencyInjection;
using Application.Services.Abstraction;
using Domain.IRepository;
using Infrastructure.DataSeed;
using Infrastructure.DependenciesInjection;
using Infrastructure.Services;
using Infrastructure.Services.EmailService;
using MEDSYstemITI.Hubs;
using MEDSYstemITI.Middleware;
//using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MEDSYstemITI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                });
            });

            builder.Services.AddinfrastructreServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSignalR();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();

            var emailConfig = builder.Configuration.GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();

                app.MapOpenApi();
            }
            
            app.UseGlobalExceptionHandling();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("DefaultCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<NotificationHub>("/notificationHub");

            // Applies pending migrations and seeds roles/permissions/admin user.
            //using (var scope = app.Services.CreateScope())
            //{
            //    await DbInitializer.SeedAsync(scope.ServiceProvider);
            //}

            app.Run();
        }
    }
}