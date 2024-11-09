using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using eShop.Catalog.API.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace eShop.Catalog.FunctionalTests;

public sealed class CatalogApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly IHost _app;

    public IResourceBuilder<PostgresServerResource> Postgres { get; private set; }
    private string _postgresConnectionString;

    public CatalogApiFixture()
    {
        var options = new DistributedApplicationOptions { AssemblyName = typeof(CatalogApiFixture).Assembly.FullName, DisableDashboard = true };
        var appBuilder = DistributedApplication.CreateBuilder(options);
        // Use a unique database name for each test run with valid characters (hyphens instead of underscores)
        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8); // Take first 8 chars for shorter name
        var dbName = $"catalogdb-{uniqueId}";
        Postgres = appBuilder.AddPostgres(dbName)
            .WithImage("ankane/pgvector")
            .WithImageTag("latest");
        _app = appBuilder.Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing"); // Prevent the original AddApplicationServices from running

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registrations
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<CatalogContext>) ||
                d.ServiceType == typeof(CatalogContext)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Add clean DbContext registration for testing
            services.AddDbContext<CatalogContext>((sp, options) =>
            {
                options.UseNpgsql(_postgresConnectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseVector();
                })
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .EnableSensitiveDataLogging();
            });

            // Add only the required seeding service
            services.AddScoped<CatalogContextSeed>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                { $"ConnectionStrings:{Postgres.Resource.Name}", _postgresConnectionString },
            });
        });
        return base.CreateHost(builder);
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _app.StopAsync();
        if (_app is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _app.Dispose();
        }
    }

    public async Task InitializeAsync()
    {
        await _app.StartAsync();

        // Wait for PostgreSQL to be ready with retry logic
        var retryCount = 0;
        const int maxRetries = 5;
        while (retryCount < maxRetries)
        {
            try
            {
                _postgresConnectionString = await Postgres.Resource.GetConnectionStringAsync();

                // Create a scope to get the DbContext and apply migrations
                using var scope = Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

                // Test the connection
                await context.Database.CanConnectAsync();

                // If we get here, the connection is successful
                // Now we can proceed with database setup
                await context.Database.MigrateAsync();

                // Seed the database
                var seeder = scope.ServiceProvider.GetRequiredService<CatalogContextSeed>();
                await seeder.SeedAsync(context);

                // If we get here, everything succeeded
                return;
            }
            catch (Exception ex)
            {
                retryCount++;
                if (retryCount == maxRetries)
                    throw new Exception($"Failed to initialize database after {maxRetries} attempts", ex);

                // Wait before retrying
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
