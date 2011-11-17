using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public class FileSystemDirectoryInfo : DirectoryInfo
    {
        private readonly string fileSystemFullPath;

        public FileSystemDirectoryInfo(ISpecialDirectoryHandler specialDirectoryHandler, string path)
            : this(specialDirectoryHandler, null, path)
        {
            
        }

        public FileSystemDirectoryInfo(ISpecialDirectoryHandler specialDirectoryHandler, DirectoryInfo parentInfo, string fileSystemFullPath)
            : base(specialDirectoryHandler, parentInfo)
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
                .Select(d => new FileSystemDirectoryInfo(SpecialDirectoryHandler, this, d))
                .Cast<IDirectoryInfo>();

            var specialDirectories = Directory.GetFiles(fileSystemFullPath)
                .Where(f => SpecialDirectoryHandler.IsSpecialDirectory(f))
                .Select(f => SpecialDirectoryHandler.GetSpecialDirectory(this, Path.GetFileName(f), () => File.OpenRead(f)));

            return directories.Union(specialDirectories);
        }

        protected override IEnumerable<IFileInfo> ExecuteGetFiles()
        {
            return Directory.GetFiles(fileSystemFullPath)
                .Where(f => !SpecialDirectoryHandler.IsSpecialDirectory(f))
                .Select(f => new FileSystemFileInfo(this, f))
                .Cast<IFileInfo>();
        }
    }
}