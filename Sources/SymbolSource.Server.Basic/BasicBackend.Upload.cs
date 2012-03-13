using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using SymbolSource.Processing.Basic;
using SymbolSource.Processing.Basic.Projects;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public void CreateJob(byte[] data, PackageProject metadata)
        {
            string directory = Path.Combine(metadata.Name, metadata.Version.Name);
            Directory.CreateDirectory(Path.Combine(configuration.DataPath, directory));

            string file = Path.Combine(configuration.DataPath, directory, metadata.Name + ".symbols.zip");
            File.WriteAllBytes(file, data);

            using (var zipMemoryStream = new MemoryStream (data))
            using (var zipfile = ZipFile.Read(zipMemoryStream))
            {
                var zipInfo = new TransformingWrapperPackageFile(new ZipPackageFile(zipfile), new UrlTransformation());
                var addInfo = addInfoBuilder.Build(zipInfo);

                string binariesDirectory = Path.Combine(directory, "Binaries");
                Directory.CreateDirectory(Path.Combine(configuration.DataPath, binariesDirectory));
                string sourcesDirectory = Path.Combine(directory, "Sources");
                Directory.CreateDirectory(Path.Combine(configuration.DataPath, sourcesDirectory));

                foreach (var binaryInfo in addInfo.Binaries)
                {
                    if (binaryInfo.SymbolInfo == null)
                        continue;

                    string binaryDirectory = Path.Combine(binariesDirectory, binaryInfo.Name, binaryInfo.SymbolHash);
                    Directory.CreateDirectory(Path.Combine(configuration.DataPath, binaryDirectory));

                    using(var binaryInfoStream = binaryInfo.File.Stream)
                    using (var binaryStream = File.OpenWrite(Path.Combine(configuration.DataPath, binaryDirectory, binaryInfo.Name + "." + binaryInfo.Type)))
                        binaryInfoStream.CopyTo(binaryStream);

                    using (var symbolInfoStream = binaryInfo.SymbolInfo.File.Stream)
                    using (var symbolStream = File.OpenWrite(Path.Combine(configuration.DataPath, binaryDirectory, binaryInfo.Name + "." + binaryInfo.SymbolInfo.Type)))
                        symbolInfoStream.CopyTo(symbolStream);

                    string indexDirectory = Path.Combine(configuration.IndexPath, binaryInfo.Name);
                    Directory.CreateDirectory(indexDirectory);

                    File.AppendAllText(Path.Combine(indexDirectory, binaryInfo.SymbolHash + ".txt"), binaryDirectory + Environment.NewLine);

                    var sourceIndex = new List<string>();

                    foreach (var sourceInfo in binaryInfo.SymbolInfo.SourceInfos)
                    {
                        string sourcePath = Path.Combine(sourcesDirectory, sourceInfo.KeyPath);
                        Directory.CreateDirectory(Path.Combine(configuration.DataPath, Path.GetDirectoryName(sourcePath)));

                        sourceIndex.Add(sourceInfo.OriginalPath + "|" + sourceInfo.KeyPath);

                        using (var sourceInfoStream = sourceInfo.ActualPath.Stream)
                        using (var sourceStream = File.OpenWrite(Path.Combine(configuration.DataPath, sourcePath)))
                            sourceInfoStream.CopyTo(sourceStream);

                        File.WriteAllLines(Path.Combine(configuration.DataPath, binaryDirectory, binaryInfo.Name + ".txt"), sourceIndex);
                    }
                }
            }

            File.Delete(file);
        }

        public void PushPackage(ref Version version, byte[] data, PackageProject metadata)
        {
            string directory = Path.Combine(configuration.DataPath, metadata.Name, metadata.Version.Name);
            Directory.CreateDirectory(directory);

            string file;
            switch(version.PackageFormat)
            {
                case "NuGet":
                    file = Path.Combine(directory, metadata.Name + "." + metadata.Version.Name + ".nupkg");
                    break;
                case "OpenWrap":
                    file = Path.Combine(directory, metadata.Name + "-" + metadata.Version.Name + ".wrap");
                    break;
                default:
                    file = Path.Combine(directory, metadata.Name + "." + version.PackageFormat);
                    break;
            }
             
            File.WriteAllBytes(file, data);
        }
    }
}