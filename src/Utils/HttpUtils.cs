using DotnetBackendSkeleton.Exceptions;

namespace DotnetBackendSkeleton.Utils
{
	public static class HttpUtils
	{
		public static async Task<T> RequestEndpoint<T>(HttpClient httpClient, HttpRequestMessage request)
		{
			HttpResponseMessage response = await RequestEndpoint(httpClient, request);

			return await response.Content.ReadFromJsonAsync<T>() ?? throw new HttpContentProcessingException($"Error when processing content returned from call to {request.RequestUri}.");
		}

		public static async Task<HttpResponseMessage> RequestEndpoint(HttpClient httpClient, HttpRequestMessage request)
		{
			try
			{
				HttpResponseMessage? response = await httpClient.SendAsync(request).ConfigureAwait(false);
				if(response is not null && response.IsSuccessStatusCode)
				{
					return response;
				}
				else
				{
					throw new HttpContentProcessingException($"Error when requesting {request.RequestUri} : {(response != null ? response.StatusCode + response.ReasonPhrase : "the response is null")}");
				}
			}
			catch(InvalidOperationException ioe)
			{
				throw new HttpInvalidRequestException($"Invalid operation {request.RequestUri}.", ioe);
			}
			catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException or InvalidOperationException)
			{
				throw new HttpUnavailableEndpointException($"Unable to reach {request.RequestUri} endpoint.", ex);
			}
			catch(Exception ex)
			{
				throw new HttpUnknownErrorException($"Unknown exception handled when calling {request.RequestUri} endpoint.", ex);
			}
		}
	}
}
