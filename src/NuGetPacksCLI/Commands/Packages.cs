using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandDotNet;
using Microsoft.Extensions.Options;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGetPacksCLI.Configurations;
using NuGetPacksCLI.Managers;
using ILogger = NuGet.Common.ILogger;

namespace NuGetPacksCLI.Commands
{
    [Command(Name = "packs",
        Usage = "packs [action] [params]",
        Description = "Work with NuGet packages")]
    public class Packages
    {
        private readonly IOptionsSnapshot<NugetManagerOptions> _opt;

        public Packages(IOptionsSnapshot<NugetManagerOptions> opt)
        {
            _opt = opt;
        }

        [Command(Name = "vers",
            Usage = "vers -n | --name <packName>",
            Description = "List available versions for target package")]
        public async Task ListAllPackVersions([Option(LongName = "name", ShortName = "n", Description = "Package name")] string packName)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = new CancellationToken();

            Console.WriteLine($"Start of search \"{packName}\" package all versions...\n");
            
            foreach (var source in _opt.Value.NugetSources.Where(it => it.IsEnabled))
            {
                var manager = new PackageManager(logger, cancellationToken);
                manager.FindAllPackageVersionsAsync(packName, source).Wait();
            }
        }

        [Command(Name = "depends",
            Usage = "depends -n | --name <packName>",
            Description = "List add dependencies of package")]
        public async Task GetPackageMeta([Option(LongName = "name", ShortName = "n", Description = "Package name")] string packName)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = new CancellationToken();

            Console.WriteLine($"Start of search \"{packName}\" package dependencies...\n");

            var manager = new PackageManager(logger, cancellationToken);
            manager.FindAllPackageDependencies(packName, _opt.Value.NugetSources.Where(it => it.IsEnabled).ToList()).Wait();
        }

        [Command(Name = "find",
            Usage = "find -p | --part <PartOfName>",
            Description = "Find packages which contains this part of name")]
        public async Task FindPackagesByPartOfName(
            [Option(LongName = "part", ShortName = "p", Description = "Part of name")] string partOnName)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = new CancellationToken();

            Console.WriteLine($"Start of search packages by part \"{partOnName}\"...");

            foreach (var source in _opt.Value.NugetSources.Where(it=>it.IsEnabled))
            {
                var manager = new PackageManager(logger, cancellationToken);
                manager.FindPackagesByPartOfName(partOnName, source).Wait();
            }
        }

        [Command(Name = "save",
            Usage = "save -n | --name <packName> (optional) -v | --version <packVersion>",
            Description = "Download package and all it dependencies")]
        public async Task DownloadPackAndAllDependencies(
            [Option(LongName = "name", ShortName = "n", Description = "Pack name")]
            string packName,
            [Option(LongName = "version", ShortName = "v", Description = "Package version")]
            string packVersion = null)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = new CancellationToken();

            Console.WriteLine($"Start downloading \"{packName}\" package dependencies...\n");

            var manager = new PackageManager(logger, cancellationToken);
            var availableSources = _opt.Value.NugetSources.Where(it => it.IsEnabled).ToList();
            manager.DownloadPackAndAllDependencies(packName, availableSources, _opt.Value.DownloadFolder).Wait();
        }
    }
}
