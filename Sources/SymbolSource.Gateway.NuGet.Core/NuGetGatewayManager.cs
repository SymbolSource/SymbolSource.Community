using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using log4net;
using NuGet;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;
using ContentType = SymbolSource.Gateway.Core.ContentType;
using MetadataEntry = SymbolSource.Server.Management.Client.MetadataEntry;
using PackageCompilation = SymbolSource.Server.Management.Client.PackageCompilation;
using PackageImageFile = SymbolSource.Server.Management.Client.PackageImageFile;
using PackageProject = SymbolSource.Server.Management.Client.PackageProject;
using PackageVersion = SymbolSource.Server.Management.Client.PackageVersion;

namespace SymbolSource.Gateway.NuGet.Core
{
    public interface INuGetGatewayManager : IGatewayManager
    {
        
    }

    public class NuGetGatewayManager : GatewayManager, INuGetGatewayManager
    {
        private readonly INuGetGatewayVersionExtractor versionExtractor;
        private static readonly ILog log = LogManager.GetLogger(typeof(NuGetGatewayManager));
        
        public NuGetGatewayManager(IGatewayBackendFactory<IPackageBackend> backendFactory, IGatewayConfigurationFactory configurationFactory, INuGetGatewayVersionExtractor versionExtractor)
            : base(backendFactory, configurationFactory)
        {
            this.versionExtractor = versionExtractor;
        }

        protected override string GetFilePath(string path)
        {
            return Path.Combine(path, "upload-1.0.nupkg");
        }

        protected override void GetMetadata(string path, string repository, out PackageProject project, out IList<MetadataEntry> metadata, out ILookup<ContentType, string> contents)
        {
            var packagePath = GetFilePath(path);

            var version = versionExtractor.Extract(packagePath);
            
            var package = new ZipPackage(packagePath);

            metadata = version.Metadata;

            project = new PackageProject
                          {
                              Name = version.Project,
                              Repository = repository,
                              Version =
                                  new PackageVersion
                                      {
                                          Project = version.Project,
                                          Name = version.Name,
                                          Metadata = version.Metadata,
                                          Compilations =
                                              package.AssemblyReferences
                                              .GroupBy(reference => reference.TargetFramework)
                                              .Select(group => new PackageCompilation
                                                                   {
                                                                       Mode = "Release",
                                                                       Platform = group.Key != null ? group.Key.ToString() : "Default",
                                                                       ImageFiles =
                                                                           group.Select(reference => new PackageImageFile
                                                                                                         {
                                                                                                             Name = reference.Path.Replace(@"\", @"/")
                                                                                                         }
                                                                           ).ToArray(),
                                                                   })

                                              .ToArray(),
                                      }

                          };


            using (var zip = new ZipFile(packagePath))
                contents = zip.EntryFileNames.ToLookup(GetContentType);
        }



        private ContentType GetContentType(string name)
        {
            var parts = name.ToLower().Split('/');

            if (parts.First() == "lib" && (parts.Last().EndsWith(".dll") || parts.Last().EndsWith(".exe") || parts.Last().EndsWith(".winmd")))
                return ContentType.Binary;

            if (parts.First() == "lib" && parts.Last().EndsWith(".xml"))
                return ContentType.Documentation;

            if (parts.First() == "lib" && parts.Last().EndsWith(".pdb"))
                return ContentType.Symbol;

            if (parts.First() == "src")
                return ContentType.Source;

            if (parts.First() != "content" && parts.Last().EndsWith(".cshtml"))
                return ContentType.Source;

            return ContentType.Other;
        }

        protected override bool? GetProjectPermission(Caller caller, string companyName, PackageProject project)
        {
            var configuration = configurationFactory.Create(companyName, project.Repository);

            if (string.IsNullOrEmpty(configuration.NuGetService))
                return null;

            return new NuGetService(configuration.NuGetService).CheckPermission(caller.KeyValue, project.Name);
        }

        protected override string GetPackageFormat()
        {
            return "NuGet";
        }
    }
}
