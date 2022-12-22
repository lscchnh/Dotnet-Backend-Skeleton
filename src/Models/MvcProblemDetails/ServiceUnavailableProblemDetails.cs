using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DotnetBackendSkeleton.Models.MvcProblemDetails;

public class ServiceUnavailableProblemDetails : ProblemDetails
{
	public ServiceUnavailableProblemDetails(string detail)
	{
		Detail = detail;
		Status = (int)HttpStatusCode.ServiceUnavailable;
		Title = "Service Unavailable";
		Type = "https://httpstatuses.com/503";
	}
}
