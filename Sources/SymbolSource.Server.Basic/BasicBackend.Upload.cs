using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using SymbolSource.Processing.Basic;
using SymbolSource.Processing.Basic.Projects;
using SymbolSource.Server.Management.Client;
using File = Delimon.Win32.IO.File;
using Directory = Delimon.Win32.IO.Directory;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public UploadReport UploadPackage(PackageProject package, string packageFormat, byte[] packageData, byte[] symbolPackageData)
        {
            if (packageData == null && symbolPackageData == null)
                throw new ArgumentNullException();

            if (packageData != null)
                PushPackage(package, packageFormat, packageData);

            if (symbolPackageData != null)
                CreateJob(package, symbolPackageData);

            return new UploadReport
                       {
                           Summary = "OK",
                       };
        }

        [Obsolete]
        private void CreateJob(PackageProject metadata, byte[] data)
        {
            string directory = Path.Combine(metadata.Name, metadata.Version.Name);
            Directory.CreateDirectory(Path.Combine(configuration.DataPath, directory));

            string file = Path.Combine(configuration.DataPath, directory, metadata.Name + ".symbols.zip");
            if (File.Exists(file))
            {
                File.Delete(file);
            }
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

                    if (binaryInfo.SymbolHash != binaryInfo.SymbolInfo.Hash)
                        throw new InvalidOperationException(string.Format("Incorrect hash code for '{0}' binaryHash '{1}' symbolHash '{2}'.", binaryInfo.Name, binaryInfo.SymbolHash, binaryInfo.SymbolInfo.Hash));

                    string binaryDirectory = Path.Combine(binariesDirectory, binaryInfo.Name, binaryInfo.SymbolHash);
                    Directory.CreateDirectory(Path.Combine(configuration.DataPath, binaryDirectory));

                    using (var binaryInfoStream = binaryInfo.File.Stream)
                    {
                        using (var binaryStream = File.OpenWrite(Path.Combine(configuration.DataPath, binaryDirectory, binaryInfo.Name + "." + binaryInfo.Type)))
                            binaryInfoStream.CopyTo(binaryStream);
                    }

                    using (var symbolInfoStream = binaryInfo.SymbolInfo.File.Stream)
                    {
                        using (var symbolStream = File.OpenWrite(Path.Combine(configuration.DataPath, binaryDirectory, binaryInfo.Name + "." + binaryInfo.SymbolInfo.Type)))
                            symbolInfoStream.CopyTo(symbolStream);
                    }

                    string indexDirectory = Path.Combine(configuration.IndexPath, binaryInfo.Name);
                    Directory.CreateDirectory(indexDirectory);

                    File.AppendAllText(Path.Combine(indexDirectory, binaryInfo.SymbolHash + ".txt"), binaryDirectory + Environment.NewLine);

                    var sourceIndex = new List<string>();

                    foreach (var sourceInfo in binaryInfo.SymbolInfo.SourceInfos.Where(info => info.ActualPath != null))
                    {
                        string sourcePath = Path.Combine(sourcesDirectory, sourceInfo.KeyPath.Replace(":/", ""));
                        Directory.CreateDirectory(Path.Combine(configuration.DataPath, Path.GetDirectoryName(sourcePath)));

                        sourceIndex.Add(sourceInfo.OriginalPath + "|" + sourceInfo.KeyPath.Replace(":/", ""));

                        using (var sourceInfoStream = sourceInfo.ActualPath.Stream)
                        {
                            using (var convertedStream = SourceConverter.Convert(sourceInfoStream))
                            {
                                using (var sourceStream = File.OpenWrite(Path.Combine(configuration.DataPath, sourcePath)))
                                    convertedStream.CopyTo(sourceStream);
                            }
                        }
                    }

                    var txtFile = Path.Combine(configuration.DataPath, binaryDirectory, binaryInfo.Name + ".txt");
                    if (File.Exists(txtFile))
                    {
                        File.Delete(txtFile);
                    }
                    File.WriteAllLines(txtFile, sourceIndex.ToArray());
                }
            }

            File.Delete(file);
        }

        [Obsolete]
        private void PushPackage(PackageProject metadata, string packageFormat, byte[] data)
        {
            var directory = Path.Combine(configuration.DataPath, metadata.Name, metadata.Version.Name);
            Directory.CreateDirectory(directory);

            var file = Path.Combine(directory, GetPackageName(packageFormat, metadata.Name, metadata.Version.Name));
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            File.WriteAllBytes(file, data);
        }

        private static string GetPackageName(string packageFormat, string projectName, string versionName)
        {
            switch (packageFormat)
            {
                case "NuGet":
                    return projectName + "." + versionName + ".nupkg";
                default:
                    throw new NotSupportedException(packageFormat);
            }
        }
    }
}