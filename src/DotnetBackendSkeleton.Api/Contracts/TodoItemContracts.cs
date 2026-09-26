using System.ComponentModel.DataAnnotations;
using DotnetBackendSkeleton.Api.Models;

namespace DotnetBackendSkeleton.Api.Contracts;

/// <summary>
/// A todo item.
/// </summary>
/// <param name="Id">The unique identifier of the item.</param>
/// <param name="Title">The title of the item.</param>
/// <param name="Status">The progress status of the item.</param>
/// <param name="CreatedAt">When the item was created (UTC).</param>
/// <param name="UpdatedAt">When the item was last updated (UTC), if ever.</param>
public sealed record TodoItemResponse(Guid Id, string Title, TodoStatus Status, DateTime CreatedAt, DateTime? UpdatedAt);

/// <summary>
/// The payload to create a todo item.
/// </summary>
public sealed record CreateTodoItemRequest
{
    /// <summary>
    /// The title of the item.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(TodoItem.TitleMaxLength)]
    public required string Title { get; init; }

    /// <summary>
    /// The initial status of the item. Defaults to <c>Todo</c>.
    /// </summary>
    public TodoStatus Status { get; init; } = TodoStatus.Todo;
}

/// <summary>
/// The payload to update a todo item.
/// </summary>
public sealed record UpdateTodoItemRequest
{
    /// <summary>
    /// The new title of the item.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(TodoItem.TitleMaxLength)]
    public required string Title { get; init; }

    /// <summary>
    /// The new status of the item.
    /// </summary>
    [Required]
    public required TodoStatus Status { get; init; }
}

public static class TodoItemMappings
{
    public static TodoItemResponse ToResponse(this TodoItem item) =>
        new(item.Id, item.Title, item.Status, item.CreatedAt, item.UpdatedAt);
}
