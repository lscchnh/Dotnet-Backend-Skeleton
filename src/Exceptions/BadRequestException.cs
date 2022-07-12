using Serilog;
using System.Runtime.Serialization;

namespace BackendSkeleton.Exceptions
{
	[Serializable]
	public class BadRequestException : Exception
	{
		public BadRequestException(string? message) : base(message)
		{
			Log.Warning(message);
		}

		protected BadRequestException(SerializationInfo info, StreamingContext context)
		{
			// No need to implement it since this class doesn't have custom properties
		}
	}
}