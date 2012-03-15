using System.Collections.Generic;
using System.Linq;
using Ionic.Zip;
using OpenFileSystem.IO;
using OpenFileSystem.IO.FileSystems.Local;
using OpenWrap;
using OpenWrap.PackageManagement;
using OpenWrap.PackageManagement.Exporters.Assemblies;
using OpenWrap.Repositories;
using OpenWrap.Runtime;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.OpenWrap.Core
{
    public interface IOpenWrapGatewayManager : IGatewayManager
    {

    }

    public class OpenWrapGatewayManager : GatewayManager, IOpenWrapGatewayManager
    {
        public OpenWrapGatewayManager(IGatewayBackendFactory<IPackageBackend> backendFactory, IGatewayConfigurationFactory configurationFactory)
            : base(backendFactory, configurationFactory)
        {
        }

        protected override string GetFilePath(string path)
        {
            return new Path(path).Combine("upload-1.0.wrap").FullPath;
        }

        protected override void GetMetadata(string path, string repository, out PackageProject project, out IList<MetadataEntry> metadata, out ILookup<ContentType, string> contents)
        {
            var assemblyExporter = new DefaultAssemblyExporter();
            var packageInfo = new FolderRepository(LocalFileSystem.Instance.GetDirectory(path)).PackagesByName.Single().Single();
            var package = packageInfo.Load();
            var descriptor = package.Descriptor;

            var exports = assemblyExporter
                .Items<Exports.IAssembly>(package, ExecutionEnvironment.Any)
                .SelectMany(group => group)
                .GroupBy(assembly => new { assembly.Profile, assembly.Platform });

            project =
                new PackageProject
                    {
                        Name = descriptor.Name,
                        Repository = repository,
                        Version =
                            new PackageVersion
                                {
                                    Name = descriptor.Version.ToString(),
                                    Compilations = exports
                                        .Select(group =>
                                                new PackageCompilation
                                                    {
                                                        Mode = "Release",
                                                        Platform = group.Key.Profile + "-" + group.Key.Platform,
                                                        ImageFiles = group
                                                            .Select(assembly =>
                                                                new PackageImageFile
                                                                    {
                                                                        Name = GetAssemblyPath(assembly.File.Path, path)
                                                                    }).ToArray(),
                                                    }).ToArray(),
                                },
                    };

            using (var zip = new ZipFile(GetFilePath(path)))
                contents = zip.EntryFileNames.ToLookup(GetContentType);

            metadata = new List<MetadataEntry>();
        }

        private ContentType GetContentType(string name)
        {
            var parts = name.ToLower().Split('/');

            if (parts.First().StartsWith("bin") && (parts.Last().EndsWith(".dll") || parts.Last().EndsWith(".exe")))
                return ContentType.Binary;

            if (parts.First().StartsWith("bin") && parts.Last().EndsWith(".xml"))
                return ContentType.Documentation;

            if (parts.First().StartsWith("bin") && parts.Last().EndsWith(".pdb"))
                return ContentType.Symbol;

            if (parts.First() == "src")
                return ContentType.Source;

            return ContentType.Other;
        }

        private string GetAssemblyPath(string assemblyPath, string rootPath)
        {
            assemblyPath = assemblyPath.Trim('/', '\\').Replace('\\', '/');
            rootPath = rootPath.Trim('/', '\\').Replace('\\', '/') + "/_cache/upload-1.0/";
            //return assemblyPath + "|" + rootPath + "|" + assemblyPath.Replace(rootPath, "");
            return assemblyPath.Replace(rootPath, "");
        }

        protected override bool? GetProjectPermission(Caller caller, string companyName, string repositoryName, string projectName)
        {
            return null;
        }

        protected override string GetPackageFormat()
        {
            return "OpenWrap";
        }
    }
}