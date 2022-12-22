using Serilog;
using System.Runtime.Serialization;

namespace DotnetBackendSkeleton.Exceptions
{
	[Serializable]
	public class HttpInvalidRequestException : Exception
	{
		public HttpInvalidRequestException(string? message) : base(message)
		{
			Log.Error($"{message}");
		}

		public HttpInvalidRequestException(string? message, Exception innerException) : base(message, innerException)
		{
			Log.Error($"{message}. Exception : {innerException.Message}");
		}

		protected HttpInvalidRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}