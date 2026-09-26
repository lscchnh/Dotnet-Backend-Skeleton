using DotnetBackendSkeleton.Api.Contracts;
using DotnetBackendSkeleton.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSkeleton.Api.Controllers;

/// <summary>
/// Manages todo items. Sample feature: replace it with your own domain.
/// </summary>
[ApiController]
[Route("api/todos")]
public class TodosController(ITodoService todoService) : ControllerBase
{
    /// <summary>
    /// Lists all todo items, oldest first.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TodoItemResponse>>(StatusCodes.Status200OK, "application/json")]
    public async Task<ActionResult<IReadOnlyList<TodoItemResponse>>> List(CancellationToken cancellationToken)
    {
        return Ok(await todoService.ListAsync(cancellationToken));
    }

    /// <summary>
    /// Gets a todo item by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:guid}", Name = nameof(Get))]
    [ProducesResponseType<TodoItemResponse>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<ActionResult<TodoItemResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var item = await todoService.GetAsync(id, cancellationToken);

        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Creates a todo item.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/todos
    ///     {
    ///        "title": "Write the README",
    ///        "status": "Todo"
    ///     }
    /// </remarks>
    /// <param name="request">The item to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType<TodoItemResponse>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public async Task<ActionResult<TodoItemResponse>> Create(CreateTodoItemRequest request, CancellationToken cancellationToken)
    {
        var item = await todoService.CreateAsync(request, cancellationToken);

        return CreatedAtRoute(nameof(Get), new { id = item.Id }, item);
    }

    /// <summary>
    /// Updates a todo item.
    /// </summary>
    /// <param name="id">The identifier of the item.</param>
    /// <param name="request">The new values of the item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<IActionResult> Update(Guid id, UpdateTodoItemRequest request, CancellationToken cancellationToken)
    {
        return await todoService.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a todo item.
    /// </summary>
    /// <param name="id">The identifier of the item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await todoService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
