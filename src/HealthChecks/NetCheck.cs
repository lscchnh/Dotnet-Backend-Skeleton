using DotnetBackendSkeleton.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Net;

namespace DotnetBackendSkeleton.HealthChecks;

public sealed class NetCheck : IHealthCheck, IDisposable
{
	public const string HealthCheckName = "backend-connection";

	private readonly HttpClient _httpClient;

	public NetCheck(IOptions<ApplicationOptions> applicationOptions)
	{
		_httpClient = new HttpClient
		{
			BaseAddress = new Uri($"{applicationOptions.Value.HttpClientAddress}"),
			Timeout = TimeSpan.FromSeconds(20)
		};
	}

	public async Task<HealthCheckResult> CheckHealthAsync(
	HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, ""), CancellationToken.None).ConfigureAwait(false);

			if(result.StatusCode.Equals(HttpStatusCode.OK))
			{
				return HealthCheckResult.Healthy();
			}
		}
		catch(Exception)
		{
			return HealthCheckResult.Unhealthy();
		}

		return HealthCheckResult.Unhealthy();
	}

	public void Dispose()
	{
		_httpClient.Dispose();
	}
}
