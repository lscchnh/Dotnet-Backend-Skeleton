using BackendSkeleton.Models;
using BackendSkeleton.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace BackendSkeleton.UnitTests
{
	[TestClass]
	public class UnitTests
	{
		[TestMethod]
		public void TestIfGetWeatherForecastReturnIsDifferentThenNull()
		{
			WeatherForecastService service = new WeatherForecastService();
			var getWeather = service.GetWeather();
			Assert.IsNotNull(getWeather);
		}

		[TestMethod]
		public void TestIfGetWeatherForecastReturnType()
		{
			WeatherForecastService service = new WeatherForecastService();
			var getWeather = service.GetWeather();
			Check.That(getWeather).IsInstanceOf<WeatherForecast[]>();
		}
	}
}
