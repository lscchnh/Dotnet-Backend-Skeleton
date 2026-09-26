namespace DotnetBackendSkeleton.Api.Models;

public class TodoItem
{
    public const int TitleMaxLength = 200;

    public Guid Id { get; set; }

    public required string Title { get; set; }

    public TodoStatus Status { get; set; } = TodoStatus.Todo;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
