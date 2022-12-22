using Serilog;
using System.Runtime.Serialization;

namespace DotnetBackendSkeleton.Exceptions
{
	[Serializable]
	public class HttpUnknownErrorException : Exception
	{
		public HttpUnknownErrorException(string? message) : base(message)
		{
			Log.Error($"{message}");
		}

		public HttpUnknownErrorException(string? message, Exception innerException) : base(message, innerException)
		{
			Log.Error($"{message}. Exception : {innerException.Message}");
		}

		protected HttpUnknownErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}