namespace AwsLambdaDotnetWebApi.Models
{
    /// <summary>
    /// Weather forecast for the day.
    /// </summary>
    public record WeatherForecast
    {
        /// <summary>
        /// Date of the forecast.
        /// </summary>
        public required DateOnly Date { get; init; }

        /// <summary>
        /// Temperature in Celcius.
        /// </summary>
        public required int TemperatureC { get; init; }

        /// <summary>
        /// Temperature in Fahrenheit.
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// Forecast summary.
        /// </summary>
        public required string Summary { get; init; }
    }
}
