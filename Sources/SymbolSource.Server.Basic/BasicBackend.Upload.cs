using System.IO;
using SymbolSource.Processing.Basic.Projects.FileInfos;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public void CreateJob(byte[] data, PackageProject metadata)
        {
            string directory = Path.Combine(configuration.DataPath, metadata.Name, metadata.Version.Name);
            Directory.CreateDirectory(directory);

            string file = Path.Combine(directory, metadata.Name + ".symbols.zip");
            File.WriteAllBytes(file, data);

            using (var zipInfo = new ZipDirectoryInfo(new InternalDirectoryInfoFactory(), file))
            {
                var addInfo = addInfoBuilder.Build(zipInfo);

                string binariesDirectory = Path.Combine(directory, "Binaries");
                Directory.CreateDirectory(binariesDirectory);


                foreach (var binaryInfo in addInfo.Binaries)
                {
                    using(var binaryInfoStream = binaryInfo.File.GetStream(FileMode.Open))
                    using(var binaryStream = File.OpenWrite(Path.Combine(binariesDirectory, binaryInfo.Name)))
                        binaryInfoStream.CopyTo(binaryStream);

                    if(binaryInfo.SymbolInfo != null)
                    {
                        using (var symbolInfoStream = binaryInfo.SymbolInfo.File.GetStream(FileMode.Open))
                        using (var symbolStream = File.OpenWrite(Path.Combine(binariesDirectory, Path.ChangeExtension(binaryInfo.Name, binaryInfo.SymbolInfo.Type))))
                            symbolInfoStream.CopyTo(symbolStream);

                        string sourcesDirectory = Path.Combine(directory, "Sources");
                        foreach (var sourceInfo in binaryInfo.SymbolInfo.SourceInfos)
                        {
                            using(var sourceInfoStream = sourceInfo.ActualPath.GetStream(FileMode.Open))
                            using(var sourceStream = File.OpenWrite(Path.Combine(sourcesDirectory, sourceInfo.KeyPath)))
                                sourceInfoStream.CopyTo(sourceStream);
                        }
                    }
                }
            }
        }

        public void PushPackage(ref Version version, byte[] data, PackageProject metadata)
        {
            string directory = Path.Combine(configuration.DataPath, metadata.Name, metadata.Version.Name);
            string file = Path.Combine(directory, metadata.Name + "." + version.PackageFormat);
            File.WriteAllBytes(file, data);
        }
    }
}