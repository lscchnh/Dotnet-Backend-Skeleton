using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace DotnetBackendSkeleton.AppHost.Tests;

/// <summary>
/// Starts the whole distributed application (API + PostgreSQL container) through the Aspire AppHost.
/// Requires a container runtime (Docker or Podman): filter them out locally with
/// <c>dotnet test --filter "TestCategory!=RequiresDocker"</c> if none is available.
/// </summary>
[TestClass]
[TestCategory("RequiresDocker")]
public sealed class ApiEndToEndTests
{
    private static readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(3);
    private static DistributedApplication _app = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext testContext)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(testContext.CancellationToken);
        cts.CancelAfter(_defaultTimeout);

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.DotnetBackendSkeleton_AppHost>(cts.Token);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());

        _app = await appHost.BuildAsync(cts.Token);
        await _app.StartAsync(cts.Token);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("api", cts.Token);
    }

    [ClassCleanup]
    public static async Task ClassCleanup() => await _app.DisposeAsync();

    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public async Task Api_is_healthy()
    {
        using var client = _app.CreateHttpClient("api");

        var response = await client.GetAsync("/health", TestContext.CancellationToken);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task Todo_items_are_persisted_in_PostgreSQL()
    {
        using var client = _app.CreateHttpClient("api");

        var createResponse = await client.PostAsJsonAsync("/api/todos", new { title = "End-to-end" }, TestContext.CancellationToken);
        Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);

        var getResponse = await client.GetAsync(createResponse.Headers.Location, TestContext.CancellationToken);
        Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
    }
}
