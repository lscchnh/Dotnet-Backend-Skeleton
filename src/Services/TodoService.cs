using DotnetBackendSkeleton.Models;
using DotnetBackendSkeleton.Repositories;

namespace DotnetBackendSkeleton.Services
{
    public class TodoService : ITodoService
    {

        private readonly TodoItemContext _dbContext;

        public TodoService(TodoItemContext context)
        {
            this._dbContext = context;
        }


        public TodoItem? GetTodoItem(Guid id)
        {
            return _dbContext.GetByIdAsync(id);
        }

        public IEnumerable<TodoItem> GetTodoItems()
        {
            return _dbContext.GetAllAsync();
        }

        public void PutTodoItem(Guid id, TodoItem todoItem)
        {
            _dbContext.UpdateAsync(todoItem);
            _dbContext.SaveChanges();
        }

        public TodoItem AddTodoItem(TodoItem todoItem)
        {
            var task = _dbContext.AddAsync(todoItem);
            _dbContext.SaveChanges();
            return task;
        }

        public void DeleteTodoItem(Guid id)
        {
            _dbContext.DeleteAsync(id);
            _dbContext.SaveChanges();
        }
    }
}
