using System;
using System.IO;
using Ionic.Zip;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public interface IDirectoryInfoFactory
    {
        IDirectoryInfo GetDirectory(string path);
    }

    public interface ISpecialDirectoryHandler
    {
        bool IsSpecialDirectory(string name);
        IDirectoryInfo GetSpecialDirectory(DirectoryInfo parentDirectory, string name, Func<Stream> opener);
    }

    public class NullSpecialDirectoryHandler : ISpecialDirectoryHandler
    {
        public bool IsSpecialDirectory(string name)
        {
            return false;
        }

        public IDirectoryInfo GetSpecialDirectory(DirectoryInfo parentDirectory, string name, Func<Stream> opener)
        {
            throw new NotImplementedException();
        }
    }

    public class InternalDirectoryInfoFactory : IDirectoryInfoFactory, ISpecialDirectoryHandler
    {
        public IDirectoryInfo GetDirectory(string path)
        {
            return new FileSystemDirectoryInfo(this, path);
        }

        public bool IsSpecialDirectory(string name)
        {
            if (Path.GetExtension(name) == ".zip")
                return true;

            return false;
        }

        public IDirectoryInfo GetSpecialDirectory(DirectoryInfo parentDirectory, string name, Func<Stream> opener)
        {
            using (var stream = opener())
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                return new ZipDirectoryInfo(this, parentDirectory, name, ZipFile.Read(memoryStream), "/");
                //TODO: return new ZipDirectoryInfo(this, parentDirectory, name, new ZipFileWithStreamCleaner(memoryStream), "/");
            }
        }
    }

    public class ExternalDirectoryInfoFactory : IDirectoryInfoFactory, ISpecialDirectoryHandler
    {
        public IDirectoryInfo GetDirectory(string path)
        {
            return new FileSystemDirectoryInfo(this, path);
        }

        public bool IsSpecialDirectory(string name)
        {
            if (Path.GetExtension(name) == ".zip")
                return true;

            return false;
        }

        public IDirectoryInfo GetSpecialDirectory(DirectoryInfo parentDirectory, string name, Func<Stream> opener)
        {
            var filePath = Path.GetTempFileName();

            using (var zipStream = opener())
            using (var fileStream = File.OpenWrite(filePath))
            {
                var extractPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                
                return new FileSystemDirectoryInfo(this, parentDirectory, extractPath);
            }
        }
    }
}