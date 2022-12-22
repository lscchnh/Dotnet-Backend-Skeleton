using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DotnetBackendSkeleton.Options
{
	/// <summary>
	/// The ApplicationOptions class
	/// </summary>
	public class ApplicationOptions
	{
		public const string Application = "Application";

		/// <summary>
		/// The connection name.
		/// </summary>
		[Required]
		public string Name { get; init; } = string.Empty;

		/// <summary>
		/// The connection string.
		/// </summary>
		[Required]
		public string ConnectionString { get; init; } = string.Empty;

		/// <summary>
		/// The client address
		/// </summary>
		[Required, Url]
		public string HttpClientAddress { get; init; } = string.Empty;

		public override string ToString()
		{
			return JsonSerializer.Serialize(this);
		}
	}
}
