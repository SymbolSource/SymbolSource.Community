using System;
using System.IO;
using System.Text;
using NuGetPackageExplorer.Types;
using SymbolSource.Processing.Basic;

namespace SymbolSource.Integration.NuGet.PackageExplorer 
{
    [PackageContentViewerMetadata(0, ".dll")]
    public class DllContentViewer : IPackageContentViewer 
    {
        public object GetView(string extension, Stream stream)
        {
            var builder = new StringBuilder();

            var store = new BinaryStoreManager();
            builder.AppendLine("DLL hash:");
            builder.AppendLine(store.ReadBinaryHash(stream));

            builder.AppendLine("");

            builder.AppendLine("PDB hash:");
            builder.AppendLine(store.ReadPdbHash(stream));

            return builder;
        }
    }
}