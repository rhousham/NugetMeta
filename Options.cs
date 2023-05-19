using CommandLine;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NugetMeta
{
    class Options
    {
        [Option('f', "File", Required = false, Default = false, HelpText = "Please supply a path to the file that will contain all the paths to the package.config files")]
        public string File { get; set; }
    }
}
