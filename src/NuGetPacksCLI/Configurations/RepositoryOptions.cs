using System.Collections.Generic;
using NuGetPacksCLI.Models;

namespace NuGetPacksCLI.Configurations
{
    public class RepositoryOptions
    {
        public List<NugetSource> NugetSources { get; set; }
    }
}