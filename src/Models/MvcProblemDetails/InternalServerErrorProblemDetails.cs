using Microsoft.AspNetCore.Mvc;

namespace BackendSkeleton.Models.MvcProblemDetails
{
	internal class InternalServerErrorProblemDetails : ProblemDetails
	{
		private string message;

		public InternalServerErrorProblemDetails(string message)
		{
			this.message = message;
		}
	}
}