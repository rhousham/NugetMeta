using CommandLine;
using NuGet;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NugetMeta
{
    internal class Program
    {
        public static Options Options;

        static async Task Main(string[] args)
        {
            //ok really want to read a text file that will get all the packages
            var result = Parser.Default
           .ParseArguments<Options>(args)
           .WithParsed(parsed => Options = parsed);

            if (result.Tag == ParserResultType.NotParsed)
            {
                // Help text requested, or parsing failed. Exit.
                return;
            }

            var packageList = GetPackages(Options.File);


            using (var progress = new ProgressBar())
            {
                for (int i = 0; i < packageList.Count(); i++)
                {
                    var pack = packageList[i];

                    progress.Report((double)i / packageList.Count());

                    ILogger logger = NullLogger.Instance;
                    CancellationToken cancellationToken = CancellationToken.None;

                    SourceCacheContext cache = new SourceCacheContext();
                    SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
                    PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>();


                    IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync(
                        pack.Name,
                        includePrerelease: false,
                        includeUnlisted: true,
                        cache,
                        logger,
                        cancellationToken);

                    var package = packages.Where(x => x.Identity.Version == pack.Version).FirstOrDefault();




                    if (package == null)
                    {
                        Console.WriteLine(pack.Name + " - " + pack.Version);
                        Console.WriteLine("Package Not Found");

                    }
                    else
                    {
                        if (package.Vulnerabilities != null)
                        {

                            Console.WriteLine(pack.Name + " - " + pack.Version);

                            foreach (var packageConf in pack.PackageFiles)
                            {
                                Console.WriteLine(packageConf);
                            }


                            foreach (var v in package.Vulnerabilities)
                            {

                                Console.WriteLine($"AdvisoryUrl: {v.AdvisoryUrl}");

                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.DarkRed;

                                if (v.Severity == 0)
                                {
                                    Console.WriteLine($"Low Severity");
                                }
                                else if (v.Severity == 1)
                                {
                                    Console.WriteLine("Moderate Severity");
                                }
                                else if (v.Severity == 2)
                                {
                                    Console.WriteLine("High Severity");
                                }
                                else if (v.Severity == 3)
                                {
                                    Console.WriteLine("Critical Severity");
                                }

                                Console.ResetColor();

                            }
                        }
                        else
                        {
                            //Console.WriteLine(pack.Name + " - " + pack.Version);
                            //Console.WriteLine("Package Ok");
                        }

                    }




                }
            }




            Console.WriteLine("Complete");




        }
    
        
        public static List<Package> GetPackages(string filename)
        {

            var retList = new List<Package>();

            Console.WriteLine("Loading files / packages to scan ");
            using (var progress = new ProgressBar())
            {

                var lines = File.ReadAllLines(filename);

                for (int i = 0; i < lines.Length; i++)
                {

                    progress.Report((double)i / lines.Length);


                    var line = lines[i].Trim();



                    if (String.IsNullOrEmpty(line) == false && line.Count() > 0)
                    {

                        if (File.Exists(line))
                        {


                            Console.WriteLine(line);

                            var document = XDocument.Load(line);
                            var reader = new PackagesConfigReader(document);
                            foreach (PackageReference package in reader.GetPackages())
                            {

                                var pack = retList.Where(x => x.Name == package.PackageIdentity.Id && x.Version == package.PackageIdentity.Version).FirstOrDefault();

                                if (pack == null)
                                {
                                    pack = new Package
                                    {
                                        Name = package.PackageIdentity.Id,
                                        Version = package.PackageIdentity.Version,
                                        PackageFiles = new List<string> { line }

                                    };

                                    retList.Add(pack);

                                }
                                else
                                {
                                    pack.PackageFiles.Add(line);
                                }


                            }

                        }
                        else
                        {
                            Console.WriteLine("File:" + line +  " Cannot be found");
                        }
                    }
                   

                }


            }
            

            return retList;




        }

    }
}
