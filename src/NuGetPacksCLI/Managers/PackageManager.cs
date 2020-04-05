using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.Common;
using NuGetPacksCLI.Models;
using System.IO;
using CommandDotNet.Rendering;
using NuGet.Packaging;

namespace NuGetPacksCLI.Managers
{
    public class PackageManager
    {
        private readonly ILogger _logger;
        private readonly CancellationToken _cancellationToken;
        private readonly string _folderForSavePacks;
        private StringBuilder _redLine = new StringBuilder("|-");

        public PackageManager(ILogger logger, CancellationToken cancellationToken, string folderForSavePacks = "")
        {
            _logger = logger;
            _cancellationToken = cancellationToken;
            _folderForSavePacks = folderForSavePacks;
        }

        public async Task FindAllPackageVersionsAsync(string packageName, NugetSource source)
        {
            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3(source.URL);
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
            var versions = (await resource.GetAllVersionsAsync(
                packageName,
                cache,
                _logger,
                _cancellationToken)).ToList().OrderByDescending(x => x).ToList();

            Console.WriteLine($"Found in: \"{source.Name}\" \npack: \"{packageName}\" \nVersions:");
            if(!versions.Any())
                Console.WriteLine($"Pack \"{packageName}\" not found");

            foreach (NuGetVersion version in versions)
            {
                Console.WriteLine($"   |-> {version}");
            }

            Console.WriteLine();
        }

        public async Task FindPackagesByPartOfName(string namePart, NugetSource source)
        {
            SourceRepository repository = Repository.Factory.GetCoreV3(source.URL);
            PackageSearchResource resource = await repository.GetResourceAsync<PackageSearchResource>();
            SearchFilter searchFilter = new SearchFilter(includePrerelease: true);

            IEnumerable<IPackageSearchMetadata> results = (await resource.SearchAsync(
                namePart,
                searchFilter,
                skip: 0,
                take: 20,
                _logger,
                _cancellationToken)).ToList();

            Console.WriteLine($"Founded packages in source \"{source.Name}\"");

            if(!results.Any())
                Console.WriteLine("  Packages not found");

            foreach (IPackageSearchMetadata result in results)
            {
                Console.WriteLine($"  Found package {result.Identity.Id} {result.Identity.Version}");
            }
        }

        public async Task DownloadPackAndAllDependencies(string packName, List<NugetSource> sources,
            string downloadedFolder, string packVer = null)
        {
            var cache = new SourceCacheContext();

        }

        public async Task FindAndDownload(string packageName, List<NugetSource> sources, SourceCacheContext cache, string packageVersion = null)
        {
            foreach (var source in sources)
            {
                var packs = await FindPackMetaInSource(packageName, source, cache);
                if (!packs.Any())
                {
                    Console.WriteLine($"Package {packageName} not found in {source.Name}");
                    continue;
                }

                var targetPack = packageVersion == null
                    ? packs.OrderByDescending(x => x.Identity.Version.OriginalVersion).FirstOrDefault()
                    : packs.FirstOrDefault(x => x.Identity.Version.OriginalVersion == packageVersion);
                if (targetPack == null)
                {
                    Console.WriteLine($"Package {packageName} not found in {source.Name}");
                    continue;
                }
                Console.WriteLine($"Package {targetPack.Title} found in {targetPack.Identity.Version.OriginalVersion}");
                Console.WriteLine($"Download start.....");
                SourceRepository repository = Repository.Factory.GetCoreV3(source.URL);
                FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();
                using MemoryStream packageStream = new MemoryStream();

                var ans = await resource.CopyNupkgToStreamAsync(
                    packageName,
                    new NuGetVersion(packageVersion),
                    packageStream,
                    cache,
                    _logger,
                    _cancellationToken);
                if (!ans)
                {
                    Console.WriteLine("Something happened during file upload...");
                    continue;
                }
                FileStream file = new FileStream("d:\\file.txt", FileMode.Create, FileAccess.Write);
                packageStream.WriteTo(file);
                file.Close();
                Console.WriteLine($"Downloaded package {packageName} {packageVersion}");
            }
        }

        private async Task<bool> DownloadPack(string packName, string packVer, NugetSource source, SourceCacheContext cache, MemoryStream packageStream)
        {
            SourceRepository repository = Repository.Factory.GetCoreV3(source.URL);
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            var asn = await resource.CopyNupkgToStreamAsync(
                packName,
                new NuGetVersion(packVer),
                packageStream,
                cache,
                _logger,
                _cancellationToken);

            if(!asn)
            {
                Console.WriteLine($"{_redLine}Not found in {source.Name}");
                return false;
            }

            Console.WriteLine($"Downloaded package {packName} {packVer}");
            return true;
        }

        public async Task FindAllPackageDependencies(string packageName, List<NugetSource> sources, string packageVersion = null)
        {
            var cache = new SourceCacheContext();

            var foundedPackages = new List<IPackageSearchMetadata>();

            foreach (var source in sources)
            {
                foundedPackages.AddRange(await FindPackMetaInSource(packageName, source, cache));
            }

            if (!foundedPackages.Any())
            {
                sources.ForEach(it => Console.WriteLine($"{_redLine}Not found in {it.Name}"));
                return;
            }

            foundedPackages = foundedPackages.Distinct().OrderByDescending(x=>x.Identity.Version).ToList();

            var package = packageVersion == null ? 
                        foundedPackages?.First()
                        : foundedPackages?.FirstOrDefault(it=>it.Identity.Version.OriginalVersion == packageVersion);
            package.DependencySets.ToList().ForEach(a =>
            {
                a.Packages.ToList().ForEach(s =>
                {
                    Console.WriteLine($"{_redLine}{s.Id} {s.VersionRange}");
                    _redLine.Insert(0, "| ");
                    FindAllPackageDependencies(s.Id, sources, s.VersionRange.MinVersion.OriginalVersion).Wait();
                    _redLine.Remove(0, 2);
                });
            });
        }

        private async Task<List<IPackageSearchMetadata>> FindPackMetaInSource(string packName, NugetSource source, SourceCacheContext cache)
        {
            var repository = Repository.Factory.GetCoreV3(source.URL);
            var resource = await repository.GetResourceAsync<PackageMetadataResource>();

            return (await resource.GetMetadataAsync(
                packName,
                includePrerelease: false,
                includeUnlisted: false,
                cache,
                _logger,
                _cancellationToken)).ToList();
        }
    }
}
