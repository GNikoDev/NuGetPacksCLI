using CommandDotNet;
using Microsoft.Extensions.Options;
using NuGetPacksCLI.Commands;
using NuGetPacksCLI.Configurations;

namespace NuGetPacksCLI.Services
{
    [Command]
    public class NugetService
    {
        private readonly IOptionsSnapshot<RepositoryOptions> _opt;

        public NugetService(IOptionsSnapshot<RepositoryOptions> opt)
        {
            _opt = opt;
        }

        [SubCommand()]
        public Packages Packages { get; set; }

        [SubCommand()]
        public Settings Settings { get; set; }
    }
}