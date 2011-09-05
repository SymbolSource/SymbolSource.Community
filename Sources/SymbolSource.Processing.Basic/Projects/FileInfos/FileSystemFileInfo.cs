using System.IO;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public class FileSystemFileInfo : FileInfo
    {
        private readonly string fileSystemFullPath;

        public FileSystemFileInfo(DirectoryInfo parentInfo, string fileSystemFullPath)
            : base(parentInfo)
        {
            this.fileSystemFullPath = fileSystemFullPath;
        }

        public override string Name
        {
            get { return Path.GetFileName(fileSystemFullPath); }
        }

        public override Stream GetStream(FileMode fileMode)
        {
            return File.Open(fileSystemFullPath, fileMode);
        }
    }
}