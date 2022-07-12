using BackendSkeleton.Exceptions;
using BackendSkeleton.Models.MvcProblemDetails;
using Hellang.Middleware.ProblemDetails;

namespace BackendSkeleton.Extensions;
public static class ProblemDetailsExtension
{
	public static void MapExceptionsToProblemDetails(
this ProblemDetailsOptions opts)
	{
		opts.Map<BadRequestException>((ex) =>
		{
			return new BadRequestProblemDetails(ex.Message);
		});

		opts.Map<Exception>((ex) =>
		{
			return new InternalServerErrorProblemDetails(ex.Message);
		});
	}

	public static void EnrichWithInstance(
	this ProblemDetailsOptions opts)
	{
		opts.OnBeforeWriteDetails = ((ctx, pr) =>
		{
			pr.Instance = $"{ctx.Request.Path}{ctx.Request.QueryString}";
		});
	}
}
