using DotnetBackendSkeleton.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotnetBackendSkeleton.IntegrationTests;

/// <summary>
/// Hosts the whole API in memory, with PostgreSQL replaced by an in-memory SQLite database,
/// so that the HTTP pipeline (routing, validation, serialization, problem details...) is tested without Docker.
/// End-to-end tests against a real PostgreSQL live in DotnetBackendSkeleton.AppHost.Tests.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Settings read while Program.cs runs must be provided through UseSetting.
        builder.UseEnvironment("Development");
        builder.UseSetting($"ConnectionStrings:{AppDbContext.ConnectionStringName}", "Host=unused");
        builder.UseSetting("Database:ApplyMigrationsOnStartup", "false");

        builder.ConfigureTestServices(services =>
        {
            // Drop the Npgsql registrations made by AddNpgsqlDbContext, then register SQLite instead.
            var npgsqlDescriptors = services
                .Where(d => d.ServiceType == typeof(AppDbContext)
                    || d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                    || d.ServiceType == typeof(DbContextOptions)
                    || (d.ServiceType.IsGenericType
                        && d.ServiceType.GenericTypeArguments.Contains(typeof(AppDbContext))
                        && d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal) == true))
                .ToList();

            foreach (var descriptor in npgsqlDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        _connection.Open();

        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

        return host;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
