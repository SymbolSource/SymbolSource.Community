This repository holds legacy SymbolSource projects.


## SymbolSource.Server.Basic

This project has been deprecated. Head over to the main [SymbolSource](https://github.com/SymbolSource/SymbolSource) repository for its successor.


## SymbolSource.Integration.NuGet.PackageExplorer
This is the NuGet Package Explorer plugin for validating symbol packages


## SymbolSource.Integration.NuGet.CommandLine
This could one day integrate symbol package validation into nuget.exe, but is only a stub at the moment.


## SymbolSource.Processing.Uninternalizer

This is a fun C# source code converter that uses NRefactory to change all protected and internal members into public ones. We use it to publish SymbolSource.Microsoft.Cci.Metadata.