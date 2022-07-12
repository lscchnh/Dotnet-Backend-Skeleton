using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace BackendSkeleton.ServiceTests
{
	[TestClass]
	public class ServiceTests : TestBase
	{
		[TestMethod]
		public async Task Call_GetWeatherForecast_Returns_Ok()
		{
			var response = await _httpClient.GetAsync("WeatherForecast");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}
	}
}