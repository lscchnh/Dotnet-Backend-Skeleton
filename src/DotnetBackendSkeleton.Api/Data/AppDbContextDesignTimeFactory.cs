using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DotnetBackendSkeleton.Api.Data;

/// <summary>
/// Used by the EF Core tools (<c>dotnet ef migrations add ...</c>) only, so that they neither need
/// to boot the whole application nor to reach a real database to generate migrations.
/// </summary>
public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=appdb")
            .Options;

        return new AppDbContext(options);
    }
}
