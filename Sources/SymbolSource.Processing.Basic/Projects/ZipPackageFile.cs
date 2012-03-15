using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;

namespace SymbolSource.Processing.Basic.Projects
{
    public class ZipPackageFile : IPackageFile
    {
        private readonly ZipFile zipFile;

        public ZipPackageFile(ZipFile zipFile)
        {
            this.zipFile = zipFile;
        }

        public IEnumerable<IPackageEntry> Entries
        {
            get
            {
                return zipFile.Entries
                    .Select(e => new ZipPackageEntry(e));

            }
        }

        private class ZipPackageEntry : IPackageEntry
        {
            private readonly ZipEntry zipEntry;

            public ZipPackageEntry(ZipEntry zipEntry)
            {
                this.zipEntry = zipEntry;
            }

            public string FullPath
            {
                get { return zipEntry.FileName; }
            }

            public Stream Stream
            {
                get
                {
                    using(var memoryStream = new MemoryStream())
                    using(var inputStream = zipEntry.OpenReader())
                    {
                        inputStream.CopyTo(memoryStream);
                        return new MemoryStream(memoryStream.ToArray());
                    }
                }
            }
        }
    }
}