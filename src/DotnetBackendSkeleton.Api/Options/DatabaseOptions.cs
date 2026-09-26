namespace DotnetBackendSkeleton.Api.Options;

/// <summary>
/// Database behavior, bound to the <c>Database</c> configuration section.
/// The connection string itself lives in <c>ConnectionStrings:appdb</c>.
/// </summary>
public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>
    /// Applies pending EF Core migrations when the application starts.
    /// Convenient locally; in production, prefer applying migrations from the delivery pipeline
    /// (idempotent SQL script or migration bundle) so that several replicas never race.
    /// </summary>
    public bool ApplyMigrationsOnStartup { get; init; }
}
