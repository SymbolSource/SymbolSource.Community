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

The tricky part is getting OpenWrap to come along, which doesn't yet have a user-friendly restore feature (or we haven't been able to figure out how to set it up). The workaround is to:

 1. [Install OpenWrap](http://www.openwrap.org): download, run, select (i).
 1. Open a new console where you'll have OpenWrap's o.exe in PATH.
 1. Add the OpenWrap beta repository: `o add-remote -name beta -href http://wraps.openwrap.org/beta/`.
 1. Update OpenWrap to 2.x: `o update-wrap openwrap -system`.
 1. Goto to you SymbolSource.Community clone.
 1. Restore OpenWrap packages: `o update-wrap openwrap -usesystem`.


## SymbolSource.Integration.NuGet.PackageExplorer
This is the NuGet Package Explorer plugin for validating symbol packages


## SymbolSource.Integration.NuGet.CommandLine
This could one day integrate symbol package validation into nuget.exe, but is only a stub at the moment.


## SymbolSource.Processing.Uninternalizer

This is a fun C# source code converter that uses NRefactory to change all protected and internal members into public ones. We use it to publish SymbolSource.Microsoft.Cci.Metadata.