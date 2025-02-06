using AwsLambdaDotnetWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace AwsLambdaDotnetWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache distributedCache) : ControllerBase
    {
        private static readonly string[] Summaries = [ "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" ];
        private readonly ILogger<WeatherForecastController> _logger = logger;
        private readonly IDistributedCache _distributedCache = distributedCache;

        /// <summary>
        /// Retrieves the weather forecast for the next 5 days.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _logger.LogInformation("Retrieving the weather forecast...");

            var cachedWeather = await _distributedCache.GetStringAsync("weather");
            if (!string.IsNullOrWhiteSpace(cachedWeather))
            {
                _logger.LogInformation("Cache hit - returning cached weather data");
                return cachedWeather
                    .Split(',')
                    .Select((w, i) => new WeatherForecast
                    {
                        Summary = w,
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                    });
            }

            _logger.LogInformation("Cache miss");

            await _distributedCache.SetStringAsync("weather", string.Join(',', Summaries));

            return Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                });
        }
    }
}
