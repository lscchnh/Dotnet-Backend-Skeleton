using BackendSkeleton.Models;

namespace BackendSkeleton.Services;

public interface IWeatherForecastService
{
	/// <summary>
	/// Gets the weather forecast.
	/// </summary>
	/// <returns>Enumerable of WeatherForecast instances.</returns>
	public IEnumerable<WeatherForecast> GetWeather();
}
