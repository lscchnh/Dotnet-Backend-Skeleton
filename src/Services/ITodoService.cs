using DotnetBackendSkeleton.Models;

namespace DotnetBackendSkeleton.Services
{
    public interface ITodoService
    {
        IEnumerable<TodoItem> GetTodoItems();

        TodoItem? GetTodoItem(Guid id);

        TodoItem AddTodoItem(TodoItem todoItem);

        void PutTodoItem(Guid id, TodoItem todoItem);

        void DeleteTodoItem(Guid id);
    }
}
