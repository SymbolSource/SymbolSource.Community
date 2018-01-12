using System.IO;
using System.Text;
using NuGetPackageExplorer.Types;
using SymbolSource.Processing.Basic;

namespace SymbolSource.Integration.NuGet.PackageExplorer 
{
    [PackageContentViewerMetadata(0, ".pdb")]
    public class SymbolContentViewer : IPackageContentViewer 
    {
        public object GetView(string extension, Stream stream)
        {
            var builder = new StringBuilder();

            var store = new SymbolStoreManager();
            builder.AppendLine("Symbol hash:");
            builder.AppendLine(store.ReadHash(stream));

            builder.AppendLine("");

            var extractor = new ManagedSourceExtractor();
            builder.AppendLine("Compiled sources:");
            var sources = extractor.ReadSources(null, stream);
            foreach (var source in sources)
                builder.AppendLine(source);

            return builder;
        }
    }
}