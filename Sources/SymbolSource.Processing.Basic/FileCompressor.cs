using System;
using System.IO;

namespace SymbolSource.Processing.Basic
{
    public class FileCompressor : IFileCompressor
    {
        private readonly ProcessFactory processFactory = new ProcessFactory();
        private readonly IPdbStoreConfig pdbStoreConfig;

        public FileCompressor(IPdbStoreConfig pdbStoreConfig)
        {
            this.pdbStoreConfig = pdbStoreConfig;
        }

        public void Compress(string source, string destination)
        {
            var process = processFactory.Create(MakeCabPath(), Path.GetFileName(source), Path.GetFileName(destination));
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(source);
            process.Start();
            process.WaitForExit();
        }

        /*
        public Stream Compress(string originalFileName, Stream inputStream)
        {
            string fileTempPath = Path.GetTempFileName();
            
            string directoryPath = Path.Combine(Path.GetTempPath(), "hss-temp-" + Path.GetRandomFileName());

            string sourceExtension = Path.GetExtension(originalFileName);
            string sourceFilePath = Path.Combine(directoryPath, originalFileName);
            string destinationExtension = sourceExtension.Remove(sourceExtension.Length - 1) + "_";
            string destinationFilePath = Path.Combine(directoryPath, Path.ChangeExtension(originalFileName, destinationExtension));

            inputStream.Seek(0, SeekOrigin.Begin);
            using (var fileStream = File.Open(sourceFilePath, FileMode.Create, FileAccess.Write))
                inputStream.CopyTo(fileStream);

            Compress(sourceFilePath, destinationFilePath);

            File.Copy(destinationFilePath, fileTempPath, true);
            File.Delete(sourceFilePath);
            File.Delete(destinationFilePath);
            Directory.Delete(directoryPath);

            return new TempFileStream(File.OpenRead(destinationFilePath), fileTempPath);
        }
        */

        private string MakeCabPath()
        {
            if (ProcessFactory.IsWindows)
                return "makecab.exe";
            else
                return Path.Combine(pdbStoreConfig.SrcSrvPath, "makecab.exe");
        }

    }
}
