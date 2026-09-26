using System.Net;
using System.Text.Json;

namespace DotnetBackendSkeleton.IntegrationTests;

[TestClass]
public sealed class PlatformEndpointsTests
{
    private static ApiFactory _factory = null!;

    public TestContext TestContext { get; set; } = null!;

    private CancellationToken CancellationToken => TestContext.CancellationToken;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _factory = new ApiFactory();

    [ClassCleanup]
    public static async Task ClassCleanup() => await _factory.DisposeAsync();

    [TestMethod]
    [DataRow("/health")]
    [DataRow("/alive")]
    public async Task Health_endpoints_report_healthy(string path)
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(path, CancellationToken);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("Healthy", await response.Content.ReadAsStringAsync(CancellationToken));
    }

    [TestMethod]
    public async Task OpenApi_document_describes_the_api()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json", CancellationToken);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(CancellationToken));
        Assert.AreEqual("Dotnet Backend Skeleton", document.RootElement.GetProperty("info").GetProperty("title").GetString());
        Assert.IsTrue(document.RootElement.GetProperty("paths").TryGetProperty("/api/todos", out _));
    }
}
