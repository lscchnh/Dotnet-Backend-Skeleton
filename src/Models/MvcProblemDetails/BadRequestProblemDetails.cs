using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BackendSkeleton.Models.MvcProblemDetails;
public class BadRequestProblemDetails : ProblemDetails
{
	public BadRequestProblemDetails(string detail)
	{
		Detail = detail;
		Status = (int)HttpStatusCode.BadRequest;
		Title = "Bad Request";
		Type = "https://httpstatuses.com/400";
	}
}