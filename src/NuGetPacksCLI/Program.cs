using System;
using System.IO;
using System.Threading.Tasks;
using CommandDotNet;
using CommandDotNet.Directives;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Protocol.Core.Types;
using NuGetPacksCLI.Services;

namespace NuGetPacksCLI
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);

            var config = LoadConfiguration();
            var services = ConfigureServices(config);
            var serviceProvider = services.BuildServiceProvider();

            try
            {
                return await new AppRunner<NugetService>().UseDefaultMiddleware()
                    .UseMicrosoftDependencyInjection(serviceProvider)
                    .RunAsync(args);
            }
            catch (FatalProtocolException e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        private static IServiceCollection ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddCustomOptions(configuration)
                    .AddCustomDI(configuration)
                    .AddCustomLogging(configuration);

            return services;
        }

        private static IConfiguration LoadConfiguration()
        {
            var confBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return confBuilder.Build();
        }
    }
}
