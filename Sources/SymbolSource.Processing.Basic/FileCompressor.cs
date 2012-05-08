using System.IO;

namespace SymbolSource.Processing.Basic
{
    public class FileCompressor : IFileCompressor
    {
        private readonly ProcessFactory processFactory = new ProcessFactory();
       
        public void Compress(string source, string destination)
        {
            var process = processFactory.Create(MakeCabPath(), Path.GetFileName(source), Path.GetFileName(destination));
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(source);
            process.Start();
            process.WaitForExit();
        }

        private string MakeCabPath()
        {
            return "makecab.exe";
        }

    }
}
