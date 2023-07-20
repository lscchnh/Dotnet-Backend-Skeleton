using DotnetBackendSkeleton.Models;
using Microsoft.EntityFrameworkCore;
using DotnetBackendSkeleton.Models.Enums;
using System.Xml.Linq;

namespace DotnetBackendSkeleton.Repositories
{
	public class TodoItemContext : DbContext
	{
		private static List<TodoItem> TodoItemsList = new List<TodoItem>
		{
			new TodoItem { Id = Guid.Parse("BF554799-962B-4475-AD2A-484B0BDFF859"), Title = "Task 1", status = Status.TODO },
			new TodoItem { Id = Guid.Parse("3380EF7A-D5EA-476E-A622-24A6DF5188A1"), Title = "Task 2", status = Status.TODO },
			new TodoItem { Id = Guid.Parse("6662C961-F251-4545-9887-DA0DBA6605C1"), Title = "Task 3", status = Status.TODO }
		};

		public TodoItemContext(DbContextOptions<TodoItemContext> options)
		: base(options)
		{
		}

		public DbSet<TodoItem> TodoItems { get; set; } = default!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TodoItem>()
				.HasData(TodoItemsList);
		}

		// Create
		public TodoItem AddAsync(TodoItem todoItem)
		{
			//_context.TodoItems.Add(todoItem);
			//await _context.SaveChangesAsync();
			TodoItems.Add(todoItem);
			return todoItem;
		}

		// Read
		public TodoItem? GetByIdAsync(Guid id)
		{
			//return await _context.TodoItems.FindAsync(id);
			return TodoItems.FirstOrDefault(t => t.Id.Equals(id));
		}

		public List<TodoItem> GetAllAsync()
		{
			return TodoItems.ToList();
			//return await _context.TodoItems.ToListAsync();
		}

		// Update
		public void UpdateAsync(TodoItem todoItem)
		{
			TodoItems.Update(todoItem);
			//_context.TodoItems.Update(todoItem);
			//await _context.SaveChangesAsync();
		}

		// Delete
		public void DeleteAsync(Guid id)
		{
			var todoItem = GetByIdAsync(id);
			if (todoItem != null)
			{
				TodoItems.Remove(todoItem);
			}
			//_context.TodoItems.Remove(todoItem);
			//await _context.SaveChangesAsync();
		}

	}
}
