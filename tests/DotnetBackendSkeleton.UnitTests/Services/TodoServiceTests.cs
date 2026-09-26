using DotnetBackendSkeleton.Api.Contracts;
using DotnetBackendSkeleton.Api.Data;
using DotnetBackendSkeleton.Api.Diagnostics;
using DotnetBackendSkeleton.Api.Models;
using DotnetBackendSkeleton.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;

namespace DotnetBackendSkeleton.UnitTests.Services;

[TestClass]
public sealed class TodoServiceTests : IDisposable
{
    private const string ApplicationName = "DotnetBackendSkeleton.Tests";

    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2026, 1, 1, 8, 0, 0, TimeSpan.Zero));
    private readonly ServiceProvider _metricsServices = new ServiceCollection().AddMetrics().BuildServiceProvider();
    private readonly AppDbContext _dbContext;
    private readonly TodoService _service;

    public TestContext TestContext { get; set; } = null!;

    public TodoServiceTests()
    {
        // SQLite in-memory is a real relational database, closer to production than the EF InMemory provider.
        _connection.Open();
        _dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);
        _dbContext.Database.EnsureCreated();

        var environment = Substitute.For<IHostEnvironment>();
        environment.ApplicationName.Returns(ApplicationName);
        var metrics = new TodoMetrics(_metricsServices.GetRequiredService<System.Diagnostics.Metrics.IMeterFactory>(), environment);

        _service = new TodoService(_dbContext, _timeProvider, metrics, NullLogger<TodoService>.Instance);
    }

    private CancellationToken CancellationToken => TestContext.CancellationToken;

    [TestMethod]
    public async Task CreateAsync_persists_the_item_with_the_current_time()
    {
        var created = await _service.CreateAsync(new CreateTodoItemRequest { Title = "Write tests" }, CancellationToken);

        var stored = await _dbContext.TodoItems.AsNoTracking().SingleAsync(CancellationToken);
        Assert.AreEqual(created.Id, stored.Id);
        Assert.AreEqual("Write tests", stored.Title);
        Assert.AreEqual(TodoStatus.Todo, stored.Status);
        Assert.AreEqual(_timeProvider.GetUtcNow().UtcDateTime, stored.CreatedAt);
        Assert.IsNull(stored.UpdatedAt);
    }

    [TestMethod]
    public async Task CreateAsync_records_the_created_metric()
    {
        using var collector = new MetricCollector<long>(
            _metricsServices.GetRequiredService<System.Diagnostics.Metrics.IMeterFactory>(), ApplicationName, "todo.items.created");

        await _service.CreateAsync(new CreateTodoItemRequest { Title = "Measure me", Status = TodoStatus.InProgress }, CancellationToken);

        var measurement = collector.GetMeasurementSnapshot().Single();
        Assert.AreEqual(1, measurement.Value);
        Assert.AreEqual("InProgress", measurement.Tags["todo.status"]);
    }

    [TestMethod]
    public async Task ListAsync_returns_items_oldest_first()
    {
        await _service.CreateAsync(new CreateTodoItemRequest { Title = "First" }, CancellationToken);
        _timeProvider.Advance(TimeSpan.FromMinutes(1));
        await _service.CreateAsync(new CreateTodoItemRequest { Title = "Second" }, CancellationToken);

        var items = await _service.ListAsync(CancellationToken);

        string[] expectedTitles = ["First", "Second"];
        CollectionAssert.AreEqual(expectedTitles, items.Select(i => i.Title).ToArray());
    }

    [TestMethod]
    public async Task GetAsync_returns_null_when_the_item_does_not_exist()
    {
        var item = await _service.GetAsync(Guid.NewGuid(), CancellationToken);

        Assert.IsNull(item);
    }

    [TestMethod]
    public async Task UpdateAsync_updates_the_item_and_its_update_time()
    {
        var created = await _service.CreateAsync(new CreateTodoItemRequest { Title = "Draft" }, CancellationToken);
        _timeProvider.Advance(TimeSpan.FromHours(1));

        var updated = await _service.UpdateAsync(
            created.Id, new UpdateTodoItemRequest { Title = "Final", Status = TodoStatus.Done }, CancellationToken);

        Assert.IsTrue(updated);
        var item = await _service.GetAsync(created.Id, CancellationToken);
        Assert.IsNotNull(item);
        Assert.AreEqual("Final", item.Title);
        Assert.AreEqual(TodoStatus.Done, item.Status);
        Assert.AreEqual(_timeProvider.GetUtcNow().UtcDateTime, item.UpdatedAt);
    }

    [TestMethod]
    public async Task UpdateAsync_returns_false_when_the_item_does_not_exist()
    {
        var updated = await _service.UpdateAsync(
            Guid.NewGuid(), new UpdateTodoItemRequest { Title = "Ghost", Status = TodoStatus.Todo }, CancellationToken);

        Assert.IsFalse(updated);
    }

    [TestMethod]
    public async Task DeleteAsync_removes_the_item()
    {
        var created = await _service.CreateAsync(new CreateTodoItemRequest { Title = "Temporary" }, CancellationToken);

        var deleted = await _service.DeleteAsync(created.Id, CancellationToken);

        Assert.IsTrue(deleted);
        Assert.IsFalse(await _dbContext.TodoItems.AnyAsync(CancellationToken));
    }

    [TestMethod]
    public async Task DeleteAsync_returns_false_when_the_item_does_not_exist()
    {
        var deleted = await _service.DeleteAsync(Guid.NewGuid(), CancellationToken);

        Assert.IsFalse(deleted);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        _metricsServices.Dispose();
    }
}
