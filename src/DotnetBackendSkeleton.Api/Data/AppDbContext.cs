using DotnetBackendSkeleton.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetBackendSkeleton.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Name of the connection string (and of the Aspire database resource).
    /// </summary>
    public const string ConnectionStringName = "appdb";

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity mappings live in Data/Configurations, one IEntityTypeConfiguration per entity.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
