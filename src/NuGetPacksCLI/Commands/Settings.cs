using System;
using System.Collections.Generic;
using System.Text;
using CommandDotNet;

namespace NuGetPacksCLI.Commands
{
    [Command(Name = "settings", Description = "Tool settings", Usage = "settings [command] [action] [params]")]
    public class Settings
    {
        [SubCommand]
        public Sources Sources { get; set; }

        [SubCommand]
        public SavePath SavePath { get; set; }
    }
}
