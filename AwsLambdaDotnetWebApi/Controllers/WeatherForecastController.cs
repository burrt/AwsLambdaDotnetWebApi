using AwsLambdaDotnetWebApi.Models;
using AwsLambdaDotnetWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace AwsLambdaDotnetWebApi.Controllers
{
    [ApiController]
    [Route("/weatherforecast")]
    public class WeatherForecastController  : ControllerBase
    // , IDistributedCache distributedCache) : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherFetcher weatherFetcher;
        private readonly IDistributedCache _distributedCache;

        public WeatherForecastController(ILogger<WeatherForecastController> logger
            // , IWeatherFetcher weatherFetcher
        )
        {
            _logger = logger;
            // this.weatherFetcher = weatherFetcher;
            // _distributedCache = distributedCache;
        }

        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];


        /// <summary>
        /// Retrieves the weather forecast for the next 5 days.
        /// </summary>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Retrieving the weather forecast...");

            if (false) {
                // var cacheValue = _distributedCache.GetString("somekey");
                // _logger.LogInformation($"Cache value: {cacheValue}");
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });
        }
    }
}
