using System.Diagnostics;
using DotnetBackendSkeleton.Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotnetBackendSkeleton.Api.Data;

/// <summary>
/// Applies pending EF Core migrations at startup when <see cref="DatabaseOptions.ApplyMigrationsOnStartup"/> is enabled.
/// Hosted services start before the server accepts requests, so the API is never served on an outdated schema.
/// </summary>
public sealed partial class DatabaseMigrator(
    IServiceScopeFactory scopeFactory,
    IOptions<DatabaseOptions> options,
    IHostEnvironment environment,
    ILogger<DatabaseMigrator> logger) : IHostedService
{
    // Named after the application so that traces are collected by AddServiceDefaults().
    private static readonly ActivitySource _activitySource = new(typeof(DatabaseMigrator).Assembly.GetName().Name!);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.ApplyMigrationsOnStartup)
        {
            return;
        }

        using var activity = _activitySource.StartActivity("Migrating database");

        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        LogApplyingMigrations(logger, environment.EnvironmentName);
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    [LoggerMessage(Level = LogLevel.Information, Message = "Applying database migrations ({Environment})")]
    private static partial void LogApplyingMigrations(ILogger logger, string environment);
}
