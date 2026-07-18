//using Application.DependencyInjection;
//using Serilog;
//using Serilog.Events;
//using System.Linq;
//using Domain.IRepository;
//using Infrastructure.DataSeed;
//using MEDSYstemITI.Middleware;
////using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using Infrastructure.DependenciesInjection;
//using Infrastructure.Services;
//using Infrastructure.Services.EmailService;
//using Application.Services.Abstraction;
//using MEDSYstemITI.Hubs;


//namespace MEDSYstemITI
//{
//    public class Program
//    {
//        public static async Task Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Configure Serilog early
//            var seqServerUrl = builder.Configuration["Serilog:SeqServerUrl"] ?? builder.Configuration["Seq:ServerUrl"];
//            Log.Logger = new LoggerConfiguration()
//                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
//                .Enrich.FromLogContext()
//                .WriteTo.Console()
//                .WriteTo.Seq(seqServerUrl ?? "http://localhost:5341")
//                .CreateLogger();

//            builder.Host.UseSerilog();

//            // Add services
//            // register filter so it can be resolved from DI (and receive ILogger via DI)


//            // Swagger
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();

//            builder.Services.AddCors(options =>
//            {
//                options.AddPolicy("DefaultCorsPolicy", policy =>
//                {
//                    policy.AllowAnyHeader()
//                          .AllowAnyMethod()
//                          .AllowAnyOrigin();
//                });
//            });

//            builder.Services.AddinfrastructreServices(builder.Configuration);
//            builder.Services.AddApplicationServices();
//            // Enable service-level logging proxies for interface-registered services
//            builder.Services.EnableServiceLogging();
//            builder.Services.AddSignalR();
//            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
//            var emailConfig = builder.Configuration.GetSection("EmailConfiguration")
//                .Get<EmailConfiguration>();
//            builder.Services.AddSingleton(emailConfig);
//            builder.Services.AddScoped<IEmailSender, EmailSender>();

//            builder.Services.AddSwaggerGen();

//            // Diagnostic: detect invalid open-generic IOptions<> registrations which cause
//            // "Open generic service type 'IOptions`1[TOptions]' requires registering an open generic implementation type." at Build().
//            var invalidOptionsRegistrations = builder.Services
//                .Where(d => d.ServiceType.IsGenericTypeDefinition
//                            && d.ServiceType.GetGenericTypeDefinition() == typeof(Microsoft.Extensions.Options.IOptions<>)
//                            && ((d.ImplementationType != null && !d.ImplementationType.IsGenericTypeDefinition)
//                                || d.ImplementationInstance != null
//                                || d.ImplementationFactory != null))
//                .ToList();

//            if (invalidOptionsRegistrations.Any())
//            {
//                Console.WriteLine("Invalid IOptions<> registrations found:");
//                foreach (var d in invalidOptionsRegistrations)
//                {
//                    Console.WriteLine($"ServiceType: {d.ServiceType}, ImplType: {d.ImplementationType}, HasInstance: {d.ImplementationInstance != null}, HasFactory: {d.ImplementationFactory != null}");
//                }
//                throw new ArgumentException("Open generic IOptions<> registered with non-open-generic implementation. See console output.");
//            }

//            var app = builder.Build();

//            // Configure HTTP pipeline
//            if (app.Environment.IsDevelopment())
//            {
//                //app.UseDeveloperExceptionPage();
//                app.UseSwagger();
//                app.UseSwaggerUI();

//                app.MapOpenApi();
//            }

//            app.UseGlobalExceptionHandling();

//            app.UseHttpsRedirection();

//            app.UseCors("DefaultCorsPolicy");

//            app.UseStaticFiles();
//            app.UseAuthentication();
//            app.UseAuthorization();
//            app.MapHub<NotificationHub>("/notificationHub");


//            app.MapControllers();

//            //using (var scope = app.Services.CreateScope())
//            //{
//            //    await DbInitializer.SeedAsync(scope.ServiceProvider);
//            //}

//            app.Run();
//        }
//    }
//}
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
            app.UseCors("DefaultCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<NotificationHub>("/notificationHub");

            //   // Applies pending migrations and seeds roles/permissions/admin user.
            using (var scope = app.Services.CreateScope())
            {
                await DbInitializer.SeedAsync(scope.ServiceProvider);
            }

            app.Run();
        }
    }
}