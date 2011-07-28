using System;
using System.IO;
using NuGetPackageExplorer.Types;

namespace SymbolSource.Integration.NuGet.PackageExplorer 
{
    [PackageContentViewerMetadata(0, ".pdb")]
    public class PdbContentViewer : IPackageContentViewer 
    {
        public object GetView(string extension, Stream stream)
        {
            return "test";
        }
    }
}