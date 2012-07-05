using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ionic.Zip;
using log4net;
using NuGet;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;
using ContentType = SymbolSource.Gateway.Core.ContentType;
using MetadataEntry = SymbolSource.Server.Management.Client.MetadataEntry;
using MetadataWrapper = SymbolSource.Server.Management.Client.MetadataWrapper;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(NuGetGatewayManager));
        
        public NuGetGatewayManager(IGatewayBackendFactory<IPackageBackend> backendFactory, IGatewayConfigurationFactory configurationFactory)
            : base(backendFactory, configurationFactory)
        {
        }

        protected override string GetFilePath(string path)
        {
            return Path.Combine(path, "upload-1.0.nupkg");
        }

        protected override void GetMetadata(string path, string repository, out PackageProject project, out IList<MetadataEntry> metadata, out ILookup<ContentType, string> contents)
        {
            var packagePath = GetFilePath(path);
            RezipIfSetConfig(packagePath);
            var package = new ZipPackage(packagePath);

            project = new PackageProject
                          {
                              Name = package.Id,
                              Repository = repository,
                              Version =
                                  new PackageVersion
                                      {
                                          Project = package.Id,
                                          Name = package.Version.ToString(),
                                          Compilations =
                                              package.AssemblyReferences
                                              .GroupBy(reference => reference.TargetFramework)
                                              .Select(group => new PackageCompilation
                                                                   {
                                                                       Mode = "Release",
                                                                       Platform =
                                                                           group.Key != null
                                                                               ? group.Key.ToString()
                                                                               : "Default",
                                                                       ImageFiles =
                                                                           group.Select(
                                                                               reference => new PackageImageFile
                                                                                                {
                                                                                                    Name =
                                                                                                        reference.Path.
                                                                                                        Replace(@"\",
                                                                                                                @"/")
                                                                                                }
                                                                           ).ToArray(),
                                                                   })

                                              .ToArray(),
                                      }

                          };

            metadata = GetMetadataEntries(package);

            using (var zip = new ZipFile(packagePath))
                contents = zip.EntryFileNames.ToLookup(GetContentType);
        }

        private static IList<MetadataEntry> GetMetadataEntries(IPackageMetadata package)
        {
            var metadata = new List<MetadataEntry>();
            var metadataWrapper = new MetadataWrapper(metadata);

            if (!package.Authors.IsEmpty())
                metadataWrapper["Authors"] = String.Join(",", package.Authors);

            if (!string.IsNullOrEmpty(package.Copyright))
                metadataWrapper["Copyrights"] = package.Copyright;

            if (!string.IsNullOrEmpty(package.Description))
                metadataWrapper["Description"] = package.Description;

            if (package.IconUrl != null)
                metadataWrapper["IconUrl"] = package.IconUrl.ToString();

            if (!string.IsNullOrEmpty(package.Language))
                metadataWrapper["Language"] = package.Language;

            if (package.LicenseUrl != null)
                metadataWrapper["LicenseUrl"] = package.LicenseUrl.ToString();

            if (!package.Owners.IsEmpty())
                metadataWrapper["Owners"] = String.Join(",", package.Owners);

            if (package.ProjectUrl != null)
                metadataWrapper["ProjectUrl"] = package.ProjectUrl.ToString();

            if (!string.IsNullOrEmpty(package.ReleaseNotes))
                metadataWrapper["ReleaseNotes"] = package.ReleaseNotes;

            metadataWrapper["RequireLicenseAcceptance"] = package.RequireLicenseAcceptance.ToString();

            if (!string.IsNullOrEmpty(package.Summary))
                metadataWrapper["Summary"] = package.Summary;

            if (!string.IsNullOrEmpty(package.Tags))
                metadataWrapper["Tags"] = package.Tags;

            if (!string.IsNullOrEmpty(package.Title))
                metadataWrapper["Title"] = package.Title;

            return metadata;
        }

        private ContentType GetContentType(string name)
        {
            var parts = name.ToLower().Split('/');

            if (parts.First() == "lib" && (parts.Last().EndsWith(".dll") || parts.Last().EndsWith(".exe")))
                return ContentType.Binary;

            if (parts.First() == "lib" && parts.Last().EndsWith(".xml"))
                return ContentType.Documentation;

            if (parts.First() == "lib" && parts.Last().EndsWith(".pdb"))
                return ContentType.Symbol;

            if (parts.First() == "src")
                return ContentType.Source;

            if (parts.Last().EndsWith(".cshtml"))
                return ContentType.Source;

            return ContentType.Other;
        }

        protected override bool? GetProjectPermission(Caller caller, string companyName, PackageProject project)
        {
            var configuration = new RepositoryConfigurationWrapper(companyName, project.Repository);

            if (string.IsNullOrEmpty(configuration.Service))
                return null;

            return new NuGetService(configuration.Service).CheckPermission(caller.KeyValue, project.Name);
        }

        protected override string GetPackageFormat()
        {
            return "NuGet";
        }

        private static void RezipIfSetConfig(string zipTempName)
        {
            if(log.IsDebugEnabled)
                log.Debug("Rezipping packages");

            string rezipPath = ConfigurationManager.AppSettings["rezip"];

            if (!string.IsNullOrEmpty(rezipPath))
            {
                if (!File.Exists(rezipPath))
                    throw new IOException(string.Format("File not exists ('{0}')", rezipPath));

                var process = Process.Start(rezipPath, zipTempName);
                process.WaitForExit();
            }
            if (log.IsDebugEnabled)
                log.Debug("Rezipped packages");
        }
    }
}
