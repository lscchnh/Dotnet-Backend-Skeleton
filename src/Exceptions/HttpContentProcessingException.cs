using Serilog;
using System.Runtime.Serialization;

namespace DotnetBackendSkeleton.Exceptions
{
	[Serializable]
	public class HttpContentProcessingException : Exception
	{
		public HttpContentProcessingException(string? message) : base(message)
		{
			Log.Error($"{message}");
		}

		public HttpContentProcessingException(string? message, Exception innerException) : base(message, innerException)
		{
			Log.Error($"{message}. Exception : {innerException.Message}");
		}

		protected HttpContentProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}