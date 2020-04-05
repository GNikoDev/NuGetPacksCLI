using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
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
            Usage = "packs vers -n | --name <packName>",
            Description = "List available versions for target package")]
        public async Task ListAllPackVersions([Option(LongName = "name", ShortName = "n", Description = "Package name")] string packName)
        {
            var availableSources = _opt.Value.NugetSources.Where(it => it.IsEnabled).ToList();
            if (!availableSources.Any())
            {
                Console.WriteLine("Source list is empty");
                return;
            }

            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = new CancellationToken();

            Console.WriteLine($"Start of search \"{packName}\" package all versions...\n");
            foreach (var source in availableSources)
            {
                var manager = new PackageManager(logger, cancellationToken);
                manager.FindAllPackageVersionsAsync(packName, source).Wait();
            }
        }

        [Command(Name = "depends",
            Usage = "packs depends -n | --name <packName>",
            Description = "List add dependencies of package")]
        public async Task GetPackageMeta([Option(LongName = "name", ShortName = "n", Description = "Package name")] string packName)
        {
            var availableSources = _opt.Value.NugetSources.Where(it => it.IsEnabled).ToList();
            if (!availableSources.Any())
            {
                Console.WriteLine("Source list is empty");
                return;
            }

            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = new CancellationToken();

            Console.WriteLine($"Start of search \"{packName}\" package dependencies...\n");
            var manager = new PackageManager(logger, cancellationToken);
            manager.FindAllPackageDependencies(packName, availableSources).Wait();
        }

        [Command(Name = "find",
            Usage = "packs find -p | --part <PartOfName>",
            Description = "Find packages which contains this part of name")]
        public async Task FindPackagesByPartOfName(
            [Option(LongName = "part", ShortName = "p", Description = "Part of name")] string partOnName)
        {
            var availableSources = _opt.Value.NugetSources.Where(it => it.IsEnabled).ToList();
            if (!availableSources.Any())
            {
                Console.WriteLine("Source list is empty");
                return;
            }

            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = new CancellationToken();

            Console.WriteLine($"Start of search packages by part \"{partOnName}\"...");
            foreach (var source in availableSources)
            {
                var manager = new PackageManager(logger, cancellationToken);
                manager.FindPackagesByPartOfName(partOnName, source).Wait();
            }
        }

        [Command(Name = "save",
            Usage = "packs save -n | --name <packName> (optional) -v | --version <packVersion>",
            Description = "Download package and all it dependencies")]
        public async Task DownloadPackAndAllDependencies(
            [Option(LongName = "name", ShortName = "n", Description = "Pack name")]
            string packName,
            [Option(LongName = "version", ShortName = "v", Description = "Package version")]
            string packVersion = null)
        {
            var availableSources = _opt.Value.NugetSources.Where(it => it.IsEnabled).ToList();
            if (!availableSources.Any())
            {
                Console.WriteLine("Source list is empty");
                return;
            }

            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = new CancellationToken();

            Console.WriteLine($"Start downloading \"{packName}\" package dependencies...\n");
            if (!Directory.Exists(_opt.Value.DownloadFolder))
                Directory.CreateDirectory(_opt.Value.DownloadFolder);
            var manager = new PackageManager(logger, cancellationToken, _opt.Value.DownloadFolder);
            manager.DownloadPackAndAllDependencies(packName, availableSources, packVersion).Wait();
        }

        [Command(Name = "savelist",
            Usage = "packs savelist <list package names>",
            Description = "Download add dependencies for all packages in selected list")]
        public async Task DownloadPacksAndDependenciesFromList(List<string> packageNames)
        {
            var availableSources = _opt.Value.NugetSources.Where(it => it.IsEnabled).ToList();
            if (!availableSources.Any())
            {
                Console.WriteLine("Source list is empty");
                return;
            }

            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = new CancellationToken();

            if(!packageNames.Any())
                Console.WriteLine("Package list is empty");
            foreach (var packageName in packageNames)
            {
                Console.WriteLine($"Start downloading \"{packageName}\" package dependencies...\n");
                if (!Directory.Exists(_opt.Value.DownloadFolder))
                    Directory.CreateDirectory(_opt.Value.DownloadFolder);
                var manager = new PackageManager(logger, cancellationToken, _opt.Value.DownloadFolder);
                manager.DownloadPackAndAllDependencies(packageName, availableSources).Wait();
            }
        }
    }
}
