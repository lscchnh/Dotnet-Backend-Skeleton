using System.Net;
using System.Net.Http.Json;
using DotnetBackendSkeleton.Api.Contracts;
using DotnetBackendSkeleton.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSkeleton.IntegrationTests;

[TestClass]
public sealed class TodosApiTests
{
    private static ApiFactory _factory = null!;
    private HttpClient _client = null!;

    public TestContext TestContext { get; set; } = null!;

    private CancellationToken CancellationToken => TestContext.CancellationToken;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _factory = new ApiFactory();

    [ClassCleanup]
    public static async Task ClassCleanup() => await _factory.DisposeAsync();

    [TestInitialize]
    public void TestInitialize() => _client = _factory.CreateClient();

    [TestCleanup]
    public void TestCleanup() => _client.Dispose();

    [TestMethod]
    public async Task Create_then_Get_returns_the_created_item()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "Integration", status = "InProgress" }, CancellationToken);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TodoItemResponse>(CancellationToken);
        Assert.IsNotNull(created);
        Assert.AreEqual(TodoStatus.InProgress, created.Status);
        Assert.AreEqual($"/api/todos/{created.Id}", response.Headers.Location?.AbsolutePath);

        var fetched = await _client.GetFromJsonAsync<TodoItemResponse>(response.Headers.Location, CancellationToken);
        Assert.AreEqual(created, fetched);
    }

    [TestMethod]
    public async Task Enums_are_serialized_as_strings()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "Json" }, CancellationToken);

        var json = await response.Content.ReadAsStringAsync(CancellationToken);
        StringAssert.Contains(json, "\"status\":\"Todo\"");
    }

    [TestMethod]
    public async Task Create_with_an_invalid_payload_returns_a_validation_problem()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "" }, CancellationToken);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(CancellationToken);
        Assert.IsNotNull(problem);
        Assert.IsTrue(problem.Errors.ContainsKey("Title"));
    }

    [TestMethod]
    public async Task Get_an_unknown_item_returns_a_not_found_problem()
    {
        var response = await _client.GetAsync($"/api/todos/{Guid.NewGuid()}", CancellationToken);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public async Task Update_then_Delete_an_item()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new { title = "Lifecycle" }, CancellationToken);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItemResponse>(CancellationToken);
        Assert.IsNotNull(created);

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/todos/{created.Id}", new { title = "Lifecycle (done)", status = "Done" }, CancellationToken);
        Assert.AreEqual(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var updated = await _client.GetFromJsonAsync<TodoItemResponse>($"/api/todos/{created.Id}", CancellationToken);
        Assert.AreEqual(TodoStatus.Done, updated?.Status);

        var deleteResponse = await _client.DeleteAsync($"/api/todos/{created.Id}", CancellationToken);
        Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var secondDeleteResponse = await _client.DeleteAsync($"/api/todos/{created.Id}", CancellationToken);
        Assert.AreEqual(HttpStatusCode.NotFound, secondDeleteResponse.StatusCode);
    }
}
