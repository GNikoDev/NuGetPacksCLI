using CommandDotNet;
using Microsoft.Extensions.Options;
using NuGetPacksCLI.Commands;
using NuGetPacksCLI.Configurations;

namespace NuGetPacksCLI.Services
{
    [Command]
    public class NugetService
    {
        private readonly IOptionsSnapshot<NugetManagerOptions> _opt;

        public NugetService(IOptionsSnapshot<NugetManagerOptions> opt)
        {
            _opt = opt;
        }

        [SubCommand()]
        public Packages Packages { get; set; }

        [SubCommand()]
        public Sources Sources { get; set; }
    }
}