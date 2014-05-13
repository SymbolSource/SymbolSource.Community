$package = $(Split-Path -parent $(Split-Path -parent $MyInvocation.MyCommand.Definition))
Copy-Item $package\lib\net40 $env:LOCALAPPDATA\NuGet\Commands\$(Split-Path -leaf $package) -recurse
