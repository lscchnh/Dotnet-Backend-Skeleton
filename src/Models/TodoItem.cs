using DotnetBackendSkeleton.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DotnetBackendSkeleton.Models
{
	public class TodoItem
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required(AllowEmptyStrings = false)]
		public string Title { get; set; } = string.Empty;

		public Status Status { get; set; } = Status.TODO;
	}
}