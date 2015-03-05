@echo off
del *.nupkg
NuGet pack SymbolSource.Server.Basic.nuspec
pause