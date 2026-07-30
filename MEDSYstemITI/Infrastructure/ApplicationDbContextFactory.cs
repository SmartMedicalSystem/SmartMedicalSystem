using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MEDSYstemITI.Infrastructure
{
    /// <summary>
    /// Tells the EF Core CLI tools (dotnet ef migrations add / database update / ...)
    /// how to create an ApplicationDbContext directly, WITHOUT booting the full
    /// WebApplication host.
    ///
    /// Why this matters: without this factory, "dotnet ef ..." falls back to
    /// building the whole app via Program.Main(). Program.Main() runs
    /// DbInitializer.SeedAsync() (which itself calls Database.MigrateAsync()
    /// and then 14 data seeders) BEFORE app.Run(). That means every single
    /// "dotnet ef" invocation was silently applying pending migrations and
    /// re-seeding the database as a side effect. If the app was already
    /// running at the same time (Visual Studio debug session, `dotnet watch`,
    /// IIS Express, a previous process that didn't exit cleanly, etc.), you'd
    /// get TWO connections concurrently issuing DDL / seed INSERTs against the
    /// same tables -> SQL Server schema-lock contention -> real deadlocks
    /// (Msg 1205), which is exactly the "migration enters a deadlock" symptom.
    ///
    /// With this factory in place, "dotnet ef" no longer touches Program.cs at
    /// all, so it can never trigger seeding or a second concurrent migration.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=(localdb)\\MSSQLLocalDB;Database=MEDSYstemITI;Trusted_Connection=True;TrustServerCertificate=true";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
