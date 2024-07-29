using AwsLambdaDotnetWebApi.Configuration;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers.Sensitive;
using System.Reflection;

namespace AwsLambdaDotnetWebApi
{
    public class Startup
    {
        public static readonly string? EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public IConfiguration Configuration { get; }

        public Startup()
        {
            Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{EnvironmentName ?? "Production"}.json", optional: true)
            .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services
                // setup configuration
                .AddSingleton(Configuration)
                .Configure<AppOptions>(Configuration.GetRequiredSection("App"))
                .Configure<SecretsManagerOptions>(Configuration.GetRequiredSection("Aws:SecretsManager"))

                // setup logging
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

                    // 
                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                })

                .AddControllers()

            // register services
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
