using BackendSkeleton.Models;
using BackendSkeleton.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BackendSkeleton.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private readonly IWeatherForecastService _weatherForecastService;

		public WeatherForecastController(IWeatherForecastService weatherForecastService)
		{
			_weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));
		}

		/// <summary>
		/// Get the weather forecast.
		/// </summary>
		/// <returns>WeatherForecast list.</returns>
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<WeatherForecast>))]
		[HttpGet(Name = "GetWeatherForecast")]
		public ActionResult<IEnumerable<WeatherForecast>> Get()
		{
			return Ok(_weatherForecastService.GetWeather());
		}
	}
}
