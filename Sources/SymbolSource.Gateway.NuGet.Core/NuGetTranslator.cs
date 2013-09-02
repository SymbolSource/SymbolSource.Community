using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using NuGet;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Gateway.NuGet.Core
{
    public static class NuGetTranslator
    {
        public static Package ConvertToPackage(Version version)
        {
            var metadataWrapper = new MetadataWrapper(version.Metadata);

            var package = new Package
                              {
                                  Id = version.Project,
                                  Version = version.Name,


                                  Title = metadataWrapper["Title"],
                                  Authors = metadataWrapper["Authors"],
                                  Copyright = metadataWrapper["Copyrights"],
                                  Description = metadataWrapper["Description"],
                                  IconUrl = metadataWrapper["IconUrl"],
                                  LicenseUrl = metadataWrapper["LicenseUrl"],
                                  ProjectUrl = metadataWrapper["ProjectUrl"],
                                  ReleaseNotes = metadataWrapper["RequireLicenseAcceptance"],
                                  RequireLicenseAcceptance = (metadataWrapper["LicenseUrl"] ?? "").Equals("true", StringComparison.OrdinalIgnoreCase),
                                  Summary = metadataWrapper["Summary"],
                                  Tags = metadataWrapper["Tags"],
                                  Dependencies = metadataWrapper["Dependencies"],
                                  PackageHash =  metadataWrapper["PackageHash"] ?? version.PackageHash,
                                  PackageHashAlgorithm = metadataWrapper["PackageHashAlgorithm"] ?? "SHA512",
                                  PackageSize = int.Parse(metadataWrapper["PackageSize"] ?? "-1"),
                                  DownloadCount = int.Parse(metadataWrapper["DownloadCount"] ?? "0"),
                                  LastUpdated = DateTime.UtcNow,
                                  Published = DateTime.UtcNow,
                                  IsLatestVersion = (metadataWrapper["IsLatestVersion"] ?? "").Equals("true", StringComparison.OrdinalIgnoreCase),
                                  IsAbsoluteLatestVersion = (metadataWrapper["IsLatestVersion"] ?? "").Equals("true", StringComparison.OrdinalIgnoreCase),
                              };

            return package;
        }

        public static Version ConvertToVersion(string path)
        {
            var package = new ZipPackage(path);           

            var version = new Version
                              {
                                  Project = package.Id,
                                  Name = package.Version.ToString(),
                              };

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

            if (package.DependencySets.SelectMany(ConvertDependencySetToStrings).Any())
                metadataWrapper["Dependencies"] = String.Join("|", package.DependencySets.SelectMany(ConvertDependencySetToStrings));

            if (!string.IsNullOrEmpty(package.Title))
                metadataWrapper["Title"] = package.Title;

            using (var stream = File.OpenRead(path))
            {
                metadataWrapper["PackageSize"] = stream.Length.ToString();
                metadataWrapper["PackageHashAlgorithm"] = "SHA512";

                stream.Seek(0, SeekOrigin.Begin);
                using (var hasher = new SHA512Managed())
                    metadataWrapper["PackageHash"] = Convert.ToBase64String(hasher.ComputeHash(stream));
            }

            metadataWrapper["DownloadCount"] = "000000";
            metadataWrapper["CreatedDate"] = DateTime.UtcNow.ToString("s");

            version.Metadata = metadata.ToArray();

            return version;
        }

        private static IEnumerable<string> ConvertDependencySetToStrings(PackageDependencySet dependencySet)
        {
            if (dependencySet.Dependencies.Count == 0)
            {
                if (dependencySet.TargetFramework != null)
                    return new[] { String.Format("::{0}", VersionUtility.GetShortFrameworkName(dependencySet.TargetFramework)) };
            }
            else
                return dependencySet.Dependencies.Select(dependency => ConvertDependency(dependency, dependencySet.TargetFramework));

            return new string[0];
        }
        private static string ConvertDependency(PackageDependency packageDependency, FrameworkName targetFramework)
        {
            if (targetFramework == null)
            {
                if (packageDependency.VersionSpec == null)
                    return packageDependency.Id;
                else
                    return String.Format("{0}:{1}", packageDependency.Id, packageDependency.VersionSpec);
            }
            else
                return String.Format("{0}:{1}:{2}", packageDependency.Id, packageDependency.VersionSpec, VersionUtility.GetShortFrameworkName(targetFramework));
        }

        public static string TranslateFilter(string toTranslate)
        {
            if(string.IsNullOrEmpty(toTranslate))
                return null;

            var trainsform = new Dictionary<string, string>
                        {
                            {"Id", "Project"},

                            {"Title", "Metadata['Title']"},
                            {"Authors", "Metadata['Authors']"},
                            {"Copyright", "Metadata['Copyrights']"},
                            {"Description", "Metadata['Description']"},
                            {"IconUrl", "Metadata['IconUrl']"},
                            {"LicenseUrl", "Metadata['LicenseUrl']"},
                            {"ProjectUrl", "Metadata['ProjectUrl']"},
                            {"ReleaseNotes", "Metadata['ReleaseNotes']"},
                            {"RequireLicenseAcceptance", "(Metadata['RequireLicenseAcceptance'] eq 'True')"},
                            {"Summary", "Metadata['Summary']"},
                            {"Tags", "Metadata['Tags']"},
                            {"Dependencies", "Metadata['Dependencies']"},
                            
                            {"Version", "Name"},

                            //Version -> Name = IsLatestName
                            {"IsLatestName", "(Metadata['IsLatestVersion'] eq 'True')"},
                            {"IsAbsoluteLatestName", "(Metadata['IsLatestVersion'] eq 'True')"},
                            //{"IsAbsoluteLatestName", "(Metadata['IsAbsoluteLatestVersion'] eq 'True')"},

                        };

            var splited = toTranslate.Split('\'');
            for(int i=0; i<splited.Length; i+=2)
                if (!string.IsNullOrEmpty(splited[i]))
                    splited[i] = trainsform.Aggregate(splited[i], (current, item) => current.Replace(item.Key, item.Value));

            return string.Join("'", splited);
        }
    }
}
