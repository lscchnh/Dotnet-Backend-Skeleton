using DotnetBackendSkeleton.Api.Contracts;
using DotnetBackendSkeleton.Api.Controllers;
using DotnetBackendSkeleton.Api.Models;
using DotnetBackendSkeleton.Api.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace DotnetBackendSkeleton.UnitTests.Controllers;

[TestClass]
public sealed class TodosControllerTests
{
    private readonly ITodoService _todoService = Substitute.For<ITodoService>();
    private readonly TodosController _controller;

    public TodosControllerTests()
    {
        _controller = new TodosController(_todoService);
    }

    [TestMethod]
    public async Task Get_returns_NotFound_when_the_item_does_not_exist()
    {
        _todoService.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((TodoItemResponse?)null);

        var result = await _controller.Get(Guid.NewGuid(), CancellationToken.None);

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    [TestMethod]
    public async Task Get_returns_Ok_with_the_item()
    {
        var item = new TodoItemResponse(Guid.NewGuid(), "Item", TodoStatus.Todo, DateTime.UtcNow, null);
        _todoService.GetAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);

        var result = await _controller.Get(item.Id, CancellationToken.None);

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        Assert.AreEqual(item, ok.Value);
    }

    [TestMethod]
    public async Task Create_returns_CreatedAtRoute_pointing_to_Get()
    {
        var request = new CreateTodoItemRequest { Title = "New" };
        var item = new TodoItemResponse(Guid.NewGuid(), "New", TodoStatus.Todo, DateTime.UtcNow, null);
        _todoService.CreateAsync(request, Arg.Any<CancellationToken>()).Returns(item);

        var result = await _controller.Create(request, CancellationToken.None);

        var created = Assert.IsInstanceOfType<CreatedAtRouteResult>(result.Result);
        Assert.AreEqual(nameof(TodosController.Get), created.RouteName);
        Assert.AreEqual(item.Id, created.RouteValues?["id"]);
        Assert.AreEqual(item, created.Value);
    }

    [TestMethod]
    public async Task Update_returns_NotFound_when_the_item_does_not_exist()
    {
        _todoService.UpdateAsync(Arg.Any<Guid>(), Arg.Any<UpdateTodoItemRequest>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _controller.Update(
            Guid.NewGuid(), new UpdateTodoItemRequest { Title = "Ghost", Status = TodoStatus.Done }, CancellationToken.None);

        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Delete_returns_NoContent_when_the_item_is_deleted()
    {
        _todoService.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await _controller.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsInstanceOfType<NoContentResult>(result);
    }
}
