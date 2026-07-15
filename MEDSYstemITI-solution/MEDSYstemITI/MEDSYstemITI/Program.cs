using Infrastructure.DependenciesInjection;
using Application.DependencyInjection;
using Infrastructure.DataSeed;
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

            app.UseCors("DefaultCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Applies pending migrations and seeds roles/permissions/admin user.
            using (var scope = app.Services.CreateScope())
            {
                await DbInitializer.SeedAsync(scope.ServiceProvider);
            }

            app.Run();
        }
    }
}
