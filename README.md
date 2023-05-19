# NugetMeta
Scan a series of nuget package files and identify any vulnerabilities

Files to create a console app which can get all the issues in your packages.
Calling is like this
NugetMeta.exe -f C:\inetpub\wwwroot\NugetMeta\NugetMeta\packages.txt

The file passed in - packages.txt - each line has the path (including filename) to the packages.config file.

Any issues are shown and the packages.config are highlighted.

# Thanks to
Microsoft for the code - https://learn.microsoft.com/en-us/nuget/reference/nuget-client-sdk#get-package-metadata
DanielSWolf for the progressbar - https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54
The team that worked on the command line parser - https://github.com/commandlineparser/commandline
