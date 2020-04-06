using System;
using System.IO;
using CommandDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGetPacksCLI.Configurations;

namespace NuGetPacksCLI.Commands
{
    [Command(Name = "savepath",
        Usage = "settings savepath [action] [params]",
        Description = "Display and edit path to save packages")]
    public class SavePath
    {
        private readonly IOptions<DownloadOptions> _opt;

        public SavePath(IConfiguration conf, IOptions<DownloadOptions> opt)
        {
            _opt = opt;
        }

        [DefaultMethod]
        public void DisplaySavePath()
        {
            Console.WriteLine("Path to save packages:");
            Console.WriteLine(_opt.Value.DownloadFolder);
        }

        [Command(Name = "new",
            Usage = "settings savepath new <newPath>",
            Description = "Set new save path")]
        public void SetNewSavePath([Option(ShortName = "p", LongName = "path", Description = "New Save path")]
            string savePath)
        {
            if (!Directory.Exists(savePath))
                Console.WriteLine($"Directory \"{savePath}\" does not exist");
            _opt.Value.DownloadFolder = savePath;
            var confFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloadsettings.json");
            File.WriteAllText(confFile, JsonConvert.SerializeObject(_opt.Value));
            Console.WriteLine("Path to download files updated");
        }
    }
}
