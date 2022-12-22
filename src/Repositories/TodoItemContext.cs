using DotnetBackendSkeleton.Models;
using DotnetBackendSkeleton.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotnetBackendSkeleton.Repositories
{
	public class TodoItemContext : DbContext
	{
		private readonly List<TodoItem> TodoItems = new List<TodoItem>
		{
			new TodoItem { Id = Guid.Parse("BF554799-962B-4475-AD2A-484B0BDFF859"), Title = "Task 1", Status = Status.TODO },
			new TodoItem { Id = Guid.Parse("3380EF7A-D5EA-476E-A622-24A6DF5188A1"), Title = "Task 2", Status = Status.TODO },
			new TodoItem { Id = Guid.Parse("6662C961-F251-4545-9887-DA0DBA6605C1"), Title = "Task 3", Status = Status.TODO }
		};

		public TodoItemContext(DbContextOptions<TodoItemContext> options) : base(options) { }

		// Create
		public async Task<TodoItem> AddAsync(TodoItem todoItem)
		{
			//_context.TodoItems.Add(todoItem);
			//await _context.SaveChangesAsync();
			await Task.CompletedTask;
			TodoItems.Add(todoItem);
			return todoItem;
		}

		// Read
		public async Task<TodoItem?> GetByIdAsync(Guid id)
		{
			//return await _context.TodoItems.FindAsync(id);
			await Task.CompletedTask;
			return TodoItems.FirstOrDefault(t => t.Id.Equals(id));
		}

		public async Task<List<TodoItem>> GetAllAsync()
		{
			await Task.CompletedTask;
			return TodoItems.ToList();
			//return await _context.TodoItems.ToListAsync();
		}

		// Update
		public async Task UpdateAsync(TodoItem todoItem)
		{
			await Task.CompletedTask;
			//_context.TodoItems.Update(todoItem);
			//await _context.SaveChangesAsync();
		}

		// Delete
		public async Task DeleteAsync(Guid id)
		{
			await Task.CompletedTask;

			var todoItem = await GetByIdAsync(id);
			//_context.TodoItems.Remove(todoItem);
			//await _context.SaveChangesAsync();
		}
	}
}
