using BackendSkeleton.Models;

namespace BackendSkeleton.Services
{
	public interface IWeatherForecastService
	{
		public IEnumerable<WeatherForecast> GetWeather();
	}
}
