using DotnetBackendSkeleton.Exceptions;
using DotnetBackendSkeleton.Models.MvcProblemDetails;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace DotnetBackendSkeleton.Extensions
{
	public static class ProblemDetailsExtension
	{
		public static void MapExceptionsToProblemDetails(
		this ProblemDetailsOptions opts)
		{
			opts.Map<HttpContentProcessingException>((ex) =>
			{
				return new InternalServerErrorProblemDetails(ex.Message);
			});

			opts.Map<HttpInvalidRequestException>((ex) =>
			{
				return new InternalServerErrorProblemDetails(ex.Message);
			});

			opts.Map<HttpUnavailableEndpointException>((ex) =>
			{
				return new ServiceUnavailableProblemDetails(ex.Message);
			});

			opts.Map<HttpUnknownErrorException>((ex) =>
			{
				return new InternalServerErrorProblemDetails(ex.Message);
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
}
