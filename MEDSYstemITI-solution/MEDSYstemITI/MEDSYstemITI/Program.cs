using Application.DependencyInjection;
using Domain.IRepository;
using Infrastructure.DataSeed;
using MEDSYstemITI.Middleware;
//using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Infrastructure.DependenciesInjection;
using Infrastructure.Services;
using Infrastructure.Services.EmailService;
using Application.Services.Abstraction;
using MEDSYstemITI.Hubs;
using MEDSYstemITI.Middleware;


namespace MEDSYstemITI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            var emailConfig = builder.Configuration.GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            // Configure HTTP pipeline
            if (app.Environment.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
                
                app.MapOpenApi();
            }

            app.UseGlobalExceptionHandling();

            app.UseHttpsRedirection();

            app.UseCors("DefaultCorsPolicy");

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<NotificationHub>("/notificationHub");


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                await DbInitializer.SeedAsync(scope.ServiceProvider);
            }

            app.Run();
        }
    }
}