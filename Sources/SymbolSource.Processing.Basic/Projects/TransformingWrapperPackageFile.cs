using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymbolSource.Processing.Basic.Projects
{
    class TransformingWrapperPackageFile : IPackageFile
    {
        private readonly IPackageFile packageFile;
        private readonly ITransformation transformation;

        public TransformingWrapperPackageFile(IPackageFile packageFile, ITransformation transformation)
        {
            this.packageFile = packageFile;
            this.transformation = transformation;
        }

        public IEnumerable<IPackageEntry> Entries
        {
            get { return packageFile.Entries.Select(e => new TransformingWrapperPackageEntry(e, transformation)); }
        }

        private class TransformingWrapperPackageEntry : IPackageEntry
        {
            private readonly IPackageEntry packageEntry;
            private readonly ITransformation transformation;

            public TransformingWrapperPackageEntry(IPackageEntry packageEntry, ITransformation transformation)
            {
                this.packageEntry = packageEntry;
                this.transformation = transformation;
            }

            public string FullPath
            {
                get { return transformation.DecodePath(packageEntry.FullPath); }
            }

            public Stream Stream
            {
                get { return packageEntry.Stream; }
            }
        }
    }
}
