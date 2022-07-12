using BackendSkeleton.Models;

namespace BackendSkeleton.Services;

public class WeatherForecastService : IWeatherForecastService
{
	private readonly static string[] Summaries = new[]
	{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	/// <inheritdoc />
	public IEnumerable<WeatherForecast> GetWeather()
	{
		return Enumerable.Range(1, 5).Select(index => new WeatherForecast
		{
			Date = DateTime.Now.AddDays(index),
			TemperatureC = Random.Shared.Next(-20, 55),
			Summary = Summaries[Random.Shared.Next(Summaries.Length)]
		})
		.ToArray();
	}
}
