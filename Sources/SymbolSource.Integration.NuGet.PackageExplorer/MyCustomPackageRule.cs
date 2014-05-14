using System;
using System.Collections.Generic;
using NuGetPackageExplorer.Types;
using System.ComponentModel.Composition;

namespace SymbolSource.Integration.NuGet.PackageExplorer 
{
    [Export(typeof(IPackageRule))]
    internal class MyCustomPackageRule : IPackageRule 
    {
        public IEnumerable<NuGetPackageExplorer.Types.PackageIssue> Validate(global::NuGet.IPackage package, string packageFileName)
        {
            throw new NotImplementedException();
        }
    }
}