$package = $(Split-Path -parent $(Split-Path -parent $MyInvocation.MyCommand.Definition))
Remove-Item $env:LOCALAPPDATA\NuGet\Commands\$(Split-Path -leaf $package) -recurse