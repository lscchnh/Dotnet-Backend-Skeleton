using DotnetBackendSkeleton.Api.Contracts;
using DotnetBackendSkeleton.Api.Data;
using DotnetBackendSkeleton.Api.Diagnostics;
using DotnetBackendSkeleton.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetBackendSkeleton.Api.Services;

public sealed partial class TodoService(
    AppDbContext dbContext,
    TimeProvider timeProvider,
    TodoMetrics metrics,
    ILogger<TodoService> logger) : ITodoService
{
    public async Task<IReadOnlyList<TodoItemResponse>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.TodoItems
            .AsNoTracking()
            .OrderBy(t => t.CreatedAt)
            .Select(t => new TodoItemResponse(t.Id, t.Title, t.Status, t.CreatedAt, t.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItemResponse?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await dbContext.TodoItems
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return item?.ToResponse();
    }

    public async Task<TodoItemResponse> CreateAsync(CreateTodoItemRequest request, CancellationToken cancellationToken)
    {
        var item = new TodoItem
        {
            Id = Guid.CreateVersion7(),
            Title = request.Title,
            Status = request.Status,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };

        dbContext.TodoItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        metrics.ItemCreated(item.Status);
        LogItemCreated(logger, item.Id);

        return item.ToResponse();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTodoItemRequest request, CancellationToken cancellationToken)
    {
        var item = await dbContext.TodoItems.FindAsync([id], cancellationToken);
        if (item is null)
        {
            return false;
        }

        var completed = item.Status != TodoStatus.Done && request.Status == TodoStatus.Done;

        item.Title = request.Title;
        item.Status = request.Status;
        item.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;

        await dbContext.SaveChangesAsync(cancellationToken);

        if (completed)
        {
            metrics.ItemCompleted();
        }

        LogItemUpdated(logger, id);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await dbContext.TodoItems
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted == 0)
        {
            return false;
        }

        LogItemDeleted(logger, id);
        return true;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Todo item {TodoItemId} created")]
    private static partial void LogItemCreated(ILogger logger, Guid todoItemId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Todo item {TodoItemId} updated")]
    private static partial void LogItemUpdated(ILogger logger, Guid todoItemId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Todo item {TodoItemId} deleted")]
    private static partial void LogItemDeleted(ILogger logger, Guid todoItemId);
}
