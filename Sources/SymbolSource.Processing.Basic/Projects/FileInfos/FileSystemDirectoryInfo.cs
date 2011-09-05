using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public class FileSystemDirectoryInfo : DirectoryInfo
    {
        private readonly string fileSystemFullPath;

        public FileSystemDirectoryInfo(string fileSystemFullPath) : this(new DirectoryInfoFactory(), null, fileSystemFullPath)
        {
            
        }

        public FileSystemDirectoryInfo(DirectoryInfoFactory directoryInfoFactory, DirectoryInfo parentInfo, string fileSystemFullPath)
            : base(directoryInfoFactory, parentInfo)
        {
            this.fileSystemFullPath = fileSystemFullPath;
        }

        public override string Name
        {
            get { return Path.GetFileName(fileSystemFullPath); }
        }

        protected override IEnumerable<IDirectoryInfo> ExecuteGetDirectories()
        {
            var directories = Directory.GetDirectories(fileSystemFullPath)
                .Select(d => new FileSystemDirectoryInfo(DirectoryInfoFactory, this, d))
                .Cast<IDirectoryInfo>();

            var specialDirectories = Directory.GetFiles(fileSystemFullPath)
                .Where(f => DirectoryInfoFactory.IsSpecialDirectory(f))
                .Select(f => DirectoryInfoFactory.GetSpecialDirectory(this, Path.GetFileName(f), File.Open(f, FileMode.Open)));

            return directories.Union(specialDirectories);
        }

        protected override IEnumerable<IFileInfo> ExecuteGetFiles()
        {
            return Directory.GetFiles(fileSystemFullPath)
                .Where(f => !DirectoryInfoFactory.IsSpecialDirectory(f))
                .Select(f => new FileSystemFileInfo(this, f))
                .Cast<IFileInfo>();
        }
    }
}