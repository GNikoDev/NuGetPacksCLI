using System.Collections.Generic;
using NuGetPacksCLI.Models;

namespace NuGetPacksCLI.Configurations
{
    public class NugetManagerOptions
    {
        public List<NugetSource> NugetSources { get; set; }
        public string DownloadFolder { get; set; }
    }
}