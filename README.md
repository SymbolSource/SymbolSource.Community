This repository holds open source SymbolSource projects.


## SymbolSource.Server.Basic

The Gateway and Server.Basic projects hold the implementation of a symbol and source server equivalent to NuGet.Server. A simple, zeroconf solution for everyone to use. It's also available as [NuGet package](http://nuget.org/packages/SymbolSource.Server.Basic), ready to install into an empty MVC 3 project.

### Installing

You can read more about Server.Basic in these blog posts, which also include instructions on how to set it up:
 
 * [Setting up your own SymbolSource Server: step-by-ste](http://xavierdecoster.azurewebsites.net/setting-up-your-own-symbolsource-server-step-by-step) by [Xavier Decoster](http://twitter.com/xavierdecoster),
 * [Releasing the community edition of SymbolSource](http://www.symbolsource.org/Public/Blog/View/2012-03-13/Releasing_the_community_edition_of_SymbolSource) by [Marcin Mikołajczak](http://twitter.com/tripleemcoder).
 
### Building

If you wish to build and debug Server.Basic, the general procedure is to:
 1. Fork and clone the repository.
 2. Build SymbolSource.Community.sln.
 3. Run SymbolSOurce.Server.Basic.Host or nuget pack SymbolSource.Server.Basic.nuspec and install as exaplained above.

 
## SymbolSource.Integration.NuGet.PackageExplorer
This is the NuGet Package Explorer plugin for validating symbol packages


## SymbolSource.Integration.NuGet.CommandLine
This could one day integrate symbol package validation into nuget.exe, but is only a stub at the moment.


## SymbolSource.Processing.Uninternalizer

This is a fun C# source code converter that uses NRefactory to change all protected and internal members into public ones. We use it to publish SymbolSource.Microsoft.Cci.Metadata.