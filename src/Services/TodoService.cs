using DotnetBackendSkeleton.Models;
using DotnetBackendSkeleton.Options;
using DotnetBackendSkeleton.Repositories;
using Microsoft.Extensions.Options;

namespace DotnetBackendSkeleton.Services;

public class TodoService : ITodoService
{
	private readonly TodoItemContext _todoItemContext;
	private readonly HttpClient _httpClient;
	private readonly IOptions<ApplicationOptions> _appOptions;

	public TodoService(TodoItemContext todoItemContext, IOptions<ApplicationOptions> appOptions, HttpClient httpClient)
	{
		_todoItemContext = todoItemContext ?? throw new ArgumentNullException(nameof(todoItemContext));
		_appOptions = appOptions ?? throw new ArgumentNullException(nameof(appOptions));
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		_httpClient.BaseAddress = new Uri(_appOptions.Value.HttpClientAddress);
	}

	public async Task<TodoItem?> GetTodoItem(Guid id)
	{
		return await _todoItemContext.GetByIdAsync(id);
	}

	public async Task<IEnumerable<TodoItem>> GetTodoItems()
	{
		return await _todoItemContext.GetAllAsync();
	}

	public async Task PutTodoItem(Guid id, TodoItem todoItem)
	{
		await _todoItemContext.UpdateAsync(todoItem);
	}

	public async Task<TodoItem> AddTodoItem(TodoItem todoItem)
	{
		return await _todoItemContext.AddAsync(todoItem);
	}

	public async Task DeleteTodoItem(Guid id)
	{
		await _todoItemContext.DeleteAsync(id);
	}
}
