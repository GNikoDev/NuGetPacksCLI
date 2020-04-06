using System;
using System.IO;
using System.Linq;
using CommandDotNet;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGetPacksCLI.Configurations;
using NuGetPacksCLI.Models;

namespace NuGetPacksCLI.Commands
{
    [Command(Name = "sources",
            Usage = "settings sources [action] [params]",
            Description = "Work with NuGet sources")]
    public class Sources
    {
        private readonly IOptionsSnapshot<RepositoryOptions> _opt;

        public Sources(IOptionsSnapshot<RepositoryOptions> opt)
        {
            _opt = opt;
        }

        [Command(Name = "list",
                Usage = "settings sources list",
                Description = "List available sources")]
        public void List()
        {
            var i = 1;
            _opt.Value.NugetSources.ForEach(it => {
                System.Console.WriteLine($"{i}. {it.Name}");
                System.Console.WriteLine($"   {it.URL}");
                System.Console.WriteLine($"   {it.IsEnabled}");
                System.Console.WriteLine();
                i++;
            });
        }

        [Command(Name = "add",
            Usage = "settings sources add -n <sourceName> -u <sourceUrl>",
            Description = "Add source to search list")]
        public void Add([Option(LongName = "name", ShortName = "n", Description = "Source name")]
            string sourceName,
            [Option(LongName = "url", ShortName = "u", Description = "URL to nuget source")]
            string nugetUrl)
        {
            _opt.Value.NugetSources.Add(new NugetSource() { Name = sourceName, URL = nugetUrl, IsEnabled = true });
            var confFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reposettings.json");
            File.WriteAllText(confFile, JsonConvert.SerializeObject(_opt.Value));
            Console.WriteLine("The list of sources has been updated");
        }

        [Command(Name = "remove",
            Usage = "settings sources remove -n <sourceName>",
            Description = "Remove source from search list")]
        public void Remove([Option(LongName = "name", ShortName = "n", Description = "Source name")]
            string sourceName)
        {
            var targetSource = _opt.Value.NugetSources.FirstOrDefault(it => it.Name == sourceName);
            if (targetSource == null)
            {
                Console.WriteLine("Source not found in source list");
                return;
            }
            _opt.Value.NugetSources.Remove(targetSource);
            var confFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reposettings.json");
            File.WriteAllText(confFile, JsonConvert.SerializeObject(_opt.Value));
            Console.WriteLine("The list of sources has been updated");
        }

        [Command(Name = "enable",
            Usage = "settings sources enable -n <sourceName>",
            Description = "Enable source")]
        public void Enable([Option(LongName = "name", ShortName = "n", Description = "Source name")]
            string sourceName)
        {
            var targetSource = _opt.Value.NugetSources.FirstOrDefault(it => it.Name == sourceName);
            if (targetSource == null)
            {
                Console.WriteLine("Source not found in source list");
                return;
            }
            if (targetSource.IsEnabled)
            {
                Console.WriteLine("Source already enabled");
                return;
            }

            targetSource.IsEnabled = true;
            var confFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reposettings.json");
            File.WriteAllText(confFile, JsonConvert.SerializeObject(_opt.Value));
            Console.WriteLine("The list of sources has been updated");
        }

        [Command(Name = "disable",
            Usage = "settings sources disable -n <sourceName>",
            Description = "Disable source")]
        public void Disable([Option(LongName = "name", ShortName = "n", Description = "Source name")]
            string sourceName)
        {
            var targetSource = _opt.Value.NugetSources.FirstOrDefault(it => it.Name == sourceName);
            if (targetSource == null)
            {
                Console.WriteLine("Source not found in source list");
                return;
            }

            if (!targetSource.IsEnabled)
            {
                Console.WriteLine("Source already disabled");
                return;
            }
            targetSource.IsEnabled = false;
            var confFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reposettings.json");
            File.WriteAllText(confFile, JsonConvert.SerializeObject(_opt.Value));
            Console.WriteLine("The list of sources has been updated");
        }
    }
}