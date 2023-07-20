using DotnetBackendSkeleton.HealthChecks;
using DotnetBackendSkeleton.Models.Enums;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace DotnetBackendSkeleton.Extensions
{
	public static class HealthChecksExtension
	{
		public static IHealthChecksBuilder AddApplicativeHealthChecks(this IHealthChecksBuilder builder)
		{
			// Add applicative healthchecks here
			return builder
				.AddCheck<NetCheck>(NetCheck.HealthCheckName, tags: new[] { HealthCheckTag.live.ToString() });
				//.AddDbContextCheck<TodoItemContext>();
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
