using CommandDotNet;
using Microsoft.Extensions.Options;
using NuGetPacksCLI.Configurations;

namespace NuGetPacksCLI.Commands
{
    [Command(Name="sources",
            Usage="sources [action] [params]",
            Description = "Work with NuGet sources")]
    public class Sources
    {
        private readonly IOptionsSnapshot<NugetManagerOptions> _opt;

        public Sources(IOptionsSnapshot<NugetManagerOptions> opt)
        {
            _opt = opt;
        }

        [Command(Name="list",
                Usage="list",
                Description="List available sources")]
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

        //[Command(Name = "add",
        //    Usage = "add -n <sourceName> -url <sourceUrl>",
        //    Description = "Add new source")]
        //public void Add([Option(LongName = "name", ShortName = "n", Description = "Name of new source")] string n,
        //                [Option(LongName = "url", ShortName = "u", Description = "url of source")] string url)
        //{
        //    var temp = _opt.Value;
        //    temp.NugetSources.Add(new NugetSource(){Name = n, URL = url, IsEnabled = true});

        //    List();
        //}
    }
}