using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Context;

namespace Testing.PresentationAPI
{
    // Runs during test host startup and ensures the SQLite schema is created
    // using the application's IServiceProvider so Identity stores are initialized
    // in the same provider graph that tests will use.
    public class TestDatabaseInitializer : IHostedService
    {
        private readonly IServiceProvider _provider;

        public TestDatabaseInitializer(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    // EnsureCreated on the SQLite in-memory test DB. Using EnsureCreated
                    // avoids migration validation failures in test environments and
                    // creates the schema based on the current model for testing.
                    await db.Database.EnsureCreatedAsync(cancellationToken);
                try
                {
                    // Diagnostic: list tables and counts
                    var conn = db.Database.GetDbConnection();
                    System.Console.WriteLine($"Test DB connection: {conn.ConnectionString}");

                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
                    using var reader = cmd.ExecuteReader();
                    System.Console.WriteLine("SQLite tables:");
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        System.Console.WriteLine(" - " + name);
                        try
                        {
                            using var countCmd = conn.CreateCommand();
                            countCmd.CommandText = $"SELECT COUNT(*) FROM \"{name}\";";
                            var cnt = countCmd.ExecuteScalar();
                            System.Console.WriteLine($"   rows: {cnt}");
                        }
                        catch (System.Exception) { }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine("TestDatabaseInitializer schema diagnostic failed: " + ex.ToString());
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("TestDatabaseInitializer EnsureCreated exception: " + ex.ToString());
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
