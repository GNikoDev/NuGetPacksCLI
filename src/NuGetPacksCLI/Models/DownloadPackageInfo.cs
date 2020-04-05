using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetPacksCLI.Models
{
    public class DownloadPackageInfo
    {
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
        public string SourceUrl { get; set; }
    }
}
