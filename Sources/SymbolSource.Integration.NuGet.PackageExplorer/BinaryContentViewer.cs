using System;
using System.IO;
using System.Text;
using Mono.Cecil;
using NuGetPackageExplorer.Types;
using SymbolSource.Processing.Basic;

namespace SymbolSource.Integration.NuGet.PackageExplorer 
{
    [PackageContentViewerMetadata(0, ".dll", ".exe")]
    public class BinaryContentViewer : IPackageContentViewer 
    {
        public object GetView(string extension, Stream stream)
        {
            var builder = new StringBuilder();

            var module = ModuleDefinition.ReadModule(stream);
            builder.AppendLine("Assembly name:");
            builder.AppendLine(module.Assembly.FullName);

            builder.AppendLine("");

            var store = new BinaryStoreManager();
            builder.AppendLine("Binary hash:");
            builder.AppendLine(store.ReadBinaryHash(stream));

            builder.AppendLine("");

            builder.AppendLine("Symbol hash:");
            builder.AppendLine(store.ReadPdbHash(stream));

            return builder;
        }
    }
}