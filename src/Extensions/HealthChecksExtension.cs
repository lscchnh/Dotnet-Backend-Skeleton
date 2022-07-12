using BackendSkeleton.Models.Enums;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace BackendSkeleton.Extensions
{
	public static class HealthChecksExtension
	{
		public static IHealthChecksBuilder AddApplicativeHealthChecks(this IHealthChecksBuilder builder)
		{
			// Add applicative healthchecks here
			return builder;
		}

		public static WebApplication MapHealthChecks(this WebApplication app)
		{
			foreach(var tag in Enum.GetNames(typeof(HealthCheckTag)))
			{
				app.MapHealthChecks($"/health/{tag}", new HealthCheckOptions()
				{
					Predicate = (check) => check.Tags.Contains(tag),
				});
			}

			return app;
		}
	}
}
