using System.IO;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public abstract class FileInfo : Info, IFileInfo
    {
        protected FileInfo(DirectoryInfo parentInfo)
            : base(parentInfo)
        {
        }

        public abstract Stream GetStream(FileMode fileMode);

        public byte[] ReadAllBytes()
        {
           using(var stream = GetStream(FileMode.Open))
           {
               byte[] result = new byte[stream.Length];
               stream.Read(result, 0, (int) stream.Length);
               return result;
           }
        }
    }
}