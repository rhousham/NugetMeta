using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetMeta
{
    public class Package
    {
        public string Name { get; set; }

        public NuGetVersion Version { get; set; }

        public List<string> PackageFiles { get; set; }


    }
}
