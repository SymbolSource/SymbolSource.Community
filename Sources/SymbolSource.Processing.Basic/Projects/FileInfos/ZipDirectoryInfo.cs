using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public class ZipDirectoryInfo : DirectoryInfo
    {
        private readonly string name;
        private readonly ZipFile zipFile;
        private readonly string path;

        public ZipDirectoryInfo(ISpecialDirectoryHandler specialDirectoryHandler, DirectoryInfo parentInfo, string name, ZipFile zipFile, string path)
            : base(specialDirectoryHandler, parentInfo)
        {
            this.name = name;
            this.zipFile = zipFile;
            this.path = path;
        }

        public ZipDirectoryInfo(ISpecialDirectoryHandler specialDirectoryHandler, string fileSystemFullPath)
            : this(specialDirectoryHandler, null, Path.GetFileName(fileSystemFullPath), new ZipFile(fileSystemFullPath), "/")
        { }

        public override string Name
        {
            get { return name; }
        }

        private bool CheckRoot(string zipEntryName)
        {
            var zipEntryNames = ("/" + zipEntryName.TrimEnd('/')).Split('/');
            var names = path.Split('/');

            bool isOk = zipEntryNames.Length == names.Length;

            if (isOk)
                for (int i = 0; i < names.Length - 1; i++)
                    isOk &= zipEntryNames[i] == names[i];

            return isOk;
        }

        private string ZipEntryName(ZipEntry zipEntry)
        {
            var z = zipEntry.FileName.Split('/').Reverse().SkipWhile(s => string.IsNullOrEmpty(s)).First();
            return z;
        }

        protected override IEnumerable<IDirectoryInfo> ExecuteGetDirectories()
        {
            var directories = zipFile
                .Where(z => z.IsDirectory)
                .Where(z => CheckRoot(z.FileName))
                .Select(z => new ZipDirectoryInfo(SpecialDirectoryHandler, this, ZipEntryName(z), zipFile, "/" + z.FileName))
                .Cast<IDirectoryInfo>();

            var specialDirectories = zipFile
                .Where(z => !z.IsDirectory)
                .Where(z => CheckRoot(z.FileName))
                .Where(z => SpecialDirectoryHandler.IsSpecialDirectory(ZipEntryName(z)))
                .Select(z => SpecialDirectoryHandler.GetSpecialDirectory(this, ZipEntryName(z), z.OpenReader));

            return directories.Union(specialDirectories);
        }

        protected override IEnumerable<IFileInfo> ExecuteGetFiles()
        {
            var files = zipFile
                .Where(z => !z.IsDirectory)
                .Where(z => CheckRoot(z.FileName))
                .Where(z => !SpecialDirectoryHandler.IsSpecialDirectory(ZipEntryName(z)))
                .Select(z => new ZipFileInfo(this, zipFile, z))
                .Cast<IFileInfo>();

            return files;
        }

        public override void Dispose()
        {
            zipFile.Dispose();
            base.Dispose();
        }

    }
}