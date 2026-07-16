using Application.DependencyInjection;
using Serilog;
using Serilog.Events;
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


namespace MEDSYstemITI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog early
            var seqServerUrl = builder.Configuration["Serilog:SeqServerUrl"] ?? builder.Configuration["Seq:ServerUrl"];
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(seqServerUrl ?? "http://localhost:5341")
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services
            // register filter so it can be resolved from DI (and receive ILogger via DI)
            builder.Services.AddScoped<Infrastructure.Middleware.LoggingActionFilter>();

            builder.Services.AddControllers(options =>
            {
                // global action filter to log every controller endpoint (resolve from DI)
                options.Filters.AddService<Infrastructure.Middleware.LoggingActionFilter>();
            });

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
            // Enable service-level logging proxies for interface-registered services
            builder.Services.EnableServiceLogging();
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