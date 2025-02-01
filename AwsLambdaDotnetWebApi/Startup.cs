using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;
using Amazon.SecretsManager.Model;
using AwsLambdaDotnetWebApi.Configuration;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Reflection;

namespace AwsLambdaDotnetWebApi
{
    public class Startup
    {
        public static readonly string? EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public static readonly  IConfiguration Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{EnvironmentName ?? "Production"}.json", optional: true)
                .Build();

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Services to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
                services.AddControllers();
            services
                // setup configuration
                .AddSingleton(Configuration)
                .Configure<AppOptions>(Configuration.GetRequiredSection("App"))
                .Configure<SecretsManagerOptions>(Configuration.GetRequiredSection("Aws:SecretsManager"))

                // setup logging
                .AddHttpLogging(logging =>
                {
                    logging.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath | HttpLoggingFields.RequestHeaders | HttpLoggingFields.Duration;
                    // don't redact header values
                    logging.RequestHeaders.Add("some-header");
                    logging.RequestBodyLogLimit = 4096;
                    logging.ResponseBodyLogLimit = 4096;
                    logging.CombineLogs = true;
                })
                .AddLogging(builder =>
                {
                    using var log = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .CreateLogger();
                    builder
                        .ClearProviders()
                        .AddSerilog(log);
                })

                // setup Swagger
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Weather API",
                        Description = "Running on AWS Lambda!"
                    });

                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection("Aws:ElastiCache")["RedisHostname"];
                options.InstanceName = EnvironmentName;

                var secretsClient = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(Configuration.GetSection("Aws")["Region"]));
                var request = new GetSecretValueRequest { SecretId = Configuration.GetSection("Aws:SecretsManager")["RedisUserSecret"] };

                System.Console.WriteLine("fetching secrets for redis pwd");
                var redisUserPassword = Task
                    .Run(async () => await secretsClient.GetSecretValueAsync(request).ConfigureAwait(false))
                    .GetAwaiter()
                    .GetResult();
                System.Console.WriteLine("finished fetching secrets for redis pwd");

                options.ConfigurationOptions = new ConfigurationOptions()
                {
                    EndPoints = new EndPointCollection { Configuration.GetSection("Aws:ElastiCache")["RedisHostname"]! },
                    User = "web-api-lambda-user",
                    Password = redisUserPassword.SecretString,
                    Ssl = true,
                    SyncTimeout = 1000,
                    AsyncTimeout = 1000,
                    ConnectTimeout = 1000,
                };
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment _)
        {
            app.UseHttpLogging();
            app.UseSerilogRequestLogging();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
