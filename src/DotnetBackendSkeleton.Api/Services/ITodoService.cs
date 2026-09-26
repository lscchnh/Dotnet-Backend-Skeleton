using DotnetBackendSkeleton.Api.Contracts;

namespace DotnetBackendSkeleton.Api.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItemResponse>> ListAsync(CancellationToken cancellationToken);

    Task<TodoItemResponse?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<TodoItemResponse> CreateAsync(CreateTodoItemRequest request, CancellationToken cancellationToken);

    /// <returns><c>false</c> if the item does not exist.</returns>
    Task<bool> UpdateAsync(Guid id, UpdateTodoItemRequest request, CancellationToken cancellationToken);

    /// <returns><c>false</c> if the item does not exist.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
