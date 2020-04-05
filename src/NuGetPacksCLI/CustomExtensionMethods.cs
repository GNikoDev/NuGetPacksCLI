using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuGetPacksCLI.Commands;
using NuGetPacksCLI.Configurations;
using NuGetPacksCLI.Services;

namespace NuGetPacksCLI
{
    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NugetManagerOptions>(configuration);

            return services;
        }

        public static IServiceCollection AddCustomDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);

            services.AddTransient<NugetService>();
            services.AddTransient<Packages>();
            services.AddTransient<Sources>();

            return services;
        }

        public static IServiceCollection AddCustomLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(logging =>
            {
                logging.AddConfiguration(configuration.GetSection("Logging"));
                logging.AddConsole();
            }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);

            return services;
        }
    }
}