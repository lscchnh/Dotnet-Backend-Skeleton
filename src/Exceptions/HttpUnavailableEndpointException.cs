using Serilog;
using System.Runtime.Serialization;

namespace DotnetBackendSkeleton.Exceptions
{
	[Serializable]
	public class HttpUnavailableEndpointException : Exception
	{
		public HttpUnavailableEndpointException(string? message) : base(message)
		{
			Log.Error($"{message}");
		}

		public HttpUnavailableEndpointException(string? message, Exception innerException) : base(message, innerException)
		{
			Log.Error($"{message}. Exception : {innerException.Message}");
		}
		protected HttpUnavailableEndpointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}