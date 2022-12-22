using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DotnetBackendSkeleton.Models.MvcProblemDetails;

public class InternalServerErrorProblemDetails : ProblemDetails
{
	public InternalServerErrorProblemDetails(string detail)
	{
		Detail = detail;
		Status = (int)HttpStatusCode.InternalServerError;
		Title = "Internal Server Error";
		Type = "https://httpstatuses.com/500";
	}
}
