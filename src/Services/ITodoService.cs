using DotnetBackendSkeleton.Models;

namespace DotnetBackendSkeleton.Services;

public interface ITodoService
{
	Task<IEnumerable<TodoItem>> GetTodoItems();

	Task<TodoItem?> GetTodoItem(Guid id);

	Task<TodoItem> AddTodoItem(TodoItem todoItem);

	Task PutTodoItem(Guid id, TodoItem todoItem);

	Task DeleteTodoItem(Guid id);
}
