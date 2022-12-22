using DotnetBackendSkeleton.Models;
using DotnetBackendSkeleton.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DotnetBackendSkeleton.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TodoController : ControllerBase
	{
		private readonly ITodoService _todoService;

		public TodoController(ITodoService todoService)
		{
			_todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
		}

		/// <summary>
		/// Get all todoitems.
		/// </summary>
		[HttpGet]
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<TodoItem>))]
		[ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
		public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
		{
			return Ok(await _todoService.GetTodoItems());
		}

		/// <summary>
		/// Gets the todoitem by id.
		/// </summary>
		/// <param name="id"></param>
		[HttpGet("{id}")]
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(TodoItem))]
		[ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
		[ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
		public async Task<ActionResult<TodoItem>> GetTodoItem(Guid id)
		{
			var todoItem = await _todoService.GetTodoItem(id);

			if(todoItem == null)
			{
				return NotFound();
			}

			return Ok(todoItem);
		}

		/// <summary>
		/// Creates a TodoItem.
		/// </summary>
		/// <param name="todoItem"></param>
		/// <returns>A newly created TodoItem</returns>
		/// <remarks>
		/// Sample request:
		///
		///     POST /Todo
		///     {
		///        "id": (guid),
		///        "name": "Item #1",
		///        "status": "TODO"
		///     }
		///
		/// </remarks>
		[HttpPost]
		[ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(TodoItem))]
		[ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
		public async Task<ActionResult<TodoItem>> AddTodoItem(TodoItem todoItem)
		{
			await _todoService.AddTodoItem(todoItem);

			return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
		}

		/// <summary>
		/// Updates a TodoItem.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="todoItem"></param>
		/// <returns></returns>
		/// <remarks>
		/// Sample request:
		///
		///     PUT /Todo/{id}
		///     {
		///        "id": (guid),
		///        "name": "Item #1",
		///        "status": "TODO"
		///     }
		///
		/// </remarks>
		[HttpPut("{id}")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
		public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
		{
			if(!id.Equals(todoItem.Id))
			{
				return BadRequest();
			}

			await _todoService.PutTodoItem(id, todoItem);

			return NoContent();
		}

		/// <summary>
		/// Deletes a specific TodoItem.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
		[ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
		public async Task<IActionResult> DeleteTodoItem(Guid id)
		{
			await _todoService.DeleteTodoItem(id);

			return NoContent();
		}
	}
}
