using System.IO;
using System.Linq;
using Ionic.Zip;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public class ZipFileInfo : FileInfo
    {
        private readonly ZipFile zipFile;
        private readonly ZipEntry zipEntry;

        public ZipFileInfo(DirectoryInfo parentInfo, ZipFile zipFile, ZipEntry zipEntry)
            : base(parentInfo)
        {
            this.zipFile = zipFile;
            this.zipEntry = zipEntry;
        }

        public override string Name
        {
            get { return zipEntry.FileName.Split('/').Last(); }
        }

        public override Stream GetStream(FileMode fileMode)
        {
            var memoryStream = new MemoryStream();
            
            using (var zipStream = zipEntry.OpenReader())
                zipStream.CopyTo(memoryStream);

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public override void Dispose()
        {
            zipFile.Dispose();
            base.Dispose();
        }
    }
};