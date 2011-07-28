using System;
using System.IO;
using NuGetPackageExplorer.Types;

namespace SymbolSource.Integration.NuGet.PackageExplorer 
{
    [PackageContentViewerMetadata(0, ".dll")]
    public class DllContentViewer : IPackageContentViewer 
    {
        public object GetView(string extension, Stream stream)
        {
            return "test";
        }
    }
}