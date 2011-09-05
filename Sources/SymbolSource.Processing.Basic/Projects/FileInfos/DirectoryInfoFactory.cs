using System.IO;
using Ionic.Zip;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public class DirectoryInfoFactory
    {
        public bool IsSpecialDirectory(string name)
        {
            if (Path.GetExtension(name) == ".zip")
                return true;
            return false;
        }

        public IDirectoryInfo GetSpecialDirectory(DirectoryInfo parentDirectory, string name, Stream stream)
        {
            using (stream)
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                return new ZipDirectoryInfo(this, parentDirectory, name, ZipFile.Read(memoryStream), "/");
                //TODO: return new ZipDirectoryInfo(this, parentDirectory, name, new ZipFileWithStreamCleaner(memoryStream), "/");
            }
        }

        //public class ZipFileWithStreamCleaner : ZipFile
        //{
        //    private readonly Stream stream;

        //    public ZipFileWithStreamCleaner(Stream stream)
        //        : base(stream)
        //    {
        //        this.stream = stream;
        //    }

        //    protected override void Dispose(bool disposing)
        //    {
        //        if (disposing)
        //            stream.Dispose();
        //        base.Dispose(disposing);
        //    }
        //}
    }
}