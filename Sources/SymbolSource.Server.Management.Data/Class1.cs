using System;
using System.Collections.Generic;
using System.Linq;

namespace SymbolSource.Server.Management.Data
{
    #region Account

    public class Caller
    {
        public string Company { get; set; }
        public string Name { get; set; }
        public string KeyValue { get; set; }
        public string KeyType { get; set; }
    }

    public class User
    {
        public string Company { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public bool CanManageCompanies { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanViewStatistics { get; set; }
    }

    public class UserKey
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class Permission
    {
        public User User { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Grant { get; set; }
    }

    public class VersionPermission : Permission
    {
        public Version Version { get; set; }
    }

    public class ProjectPermission : Permission
    {
        public Project Project { get; set; }
    }

    public class RepositoryPermission : Permission
    {
        public Repository Repository { get; set; }
    }

    public class CompanyPermission : Permission
    {
        public Company Company { get; set; }
    }

    public class Permissions
    {
        public VersionPermission[] Versions { get; set; }
        public ProjectPermission[] Projects { get; set; }
        public RepositoryPermission[] Repositories { get; set; }
        public CompanyPermission[] Companies { get; set; }
    }

    #endregion

    #region Metadata

    public class Company
    {
        public Company()
        {
            Metadata = new List<MetadataEntry>();
        }

        public string Name { get; set; }

        public List<MetadataEntry> Metadata { get; set; }

        public bool CanCreateRepository { get; set; }
        public bool CanGrantPermission { get; set; }
    }

    public class Repository
    {
        public Repository()
        {
            Metadata = new List<MetadataEntry>();
        }

        public string Name { get; set; }
        public string Company { get; set; }

        public List<MetadataEntry> Metadata { get; set; }

        public bool CanCreateProject { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanGrantPermission { get; set; }
    }

    public class Project
    {
        public Project()
        {
            Metadata = new List<MetadataEntry>();
        }

        public string Name { get; set; }
        public string Repository { get; set; }
        public string Company { get; set; }

        public List<MetadataEntry> Metadata { get; set; }

        public bool CanCreateVersion { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanGrantPermission { get; set; }
    }

    public class Version
    {
        public Version()
        {
            Metadata = new List<MetadataEntry>();
        }

        public string Name { get; set; }
        public string Project { get; set; }
        public string Repository { get; set; }
        public string Company { get; set; }

        public string PackageFormat { get; set; }
        public string PackageHash { get; set; }
        public bool Hidden { get; set; }

        public List<MetadataEntry> Metadata { get; set; }

        public bool CanCreateCompilation { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanHide { get; set; }
        public bool CanRestore { get; set; }
        public bool CanGrantPermission { get; set; }

    }

    public class Compilation
    {
        public Compilation()
        {
            Metadata = new List<MetadataEntry>();
        }

        public string Mode { get; set; }
        public string Profile { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public string Project { get; set; }
        public string Repository { get; set; }
        public string Company { get; set; }

        public List<MetadataEntry> Metadata { get; set; }
    }

    public class ImageFile
    {
        public string Name { get; set; }
        public string FileVersion { get; set; }
        public string BinaryHash { get; set; }
        public string BinaryType { get; set; }
        public string SymbolHash { get; set; }
        public string SymbolType { get; set; }
        public string DocumentationType { get; set; }

        public Reference Reference { get; set; }
        public string Mode { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public string Project { get; set; }
        public string Repository { get; set; }
        public string Company { get; set; }
    }

    public class SourceFile
    {
        public string Path { get; set; }
        public string Hash { get; set; }
        public string OriginalPath { get; set; }

        public string ImageName { get; set; }
        public string Mode { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public string Project { get; set; }
        public string Repository { get; set; }
        public string Company { get; set; }
    }

    public class NodeImageFile
    {
        public string Name { get; set; }
        public string Fullname { get; set; }
        public string Type { get; set; }
        public string Modifier { get; set; }
        public CodeBlockAddress[] CodeBlockAddresses { get; set; }

        public string ImageName { get; set; }
        public string Mode { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public string Project { get; set; }
        public string Repository { get; set; }
        public string Company { get; set; }
    }

    public class CodeBlockAddress
    {
        public SourceFile SourceFile { get; set; }
        public Location StartPosition { get; set; }
        public Location EndPosition { get; set; }
    }

    public class Location
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public class Reference
    {
        public string Culture { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string PublicKeyToken { get; set; }
    }

    public class Depedency : Reference
    {
        public string Type { get; set; }
    }

    public class PackageFilter
    {
        public string Where { get; set; }
        public string OrderBy { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool Count { get; set; }

        public bool Performed { get; set; }
    }

    #endregion

    #region Upload

    public class UploadReport
    {
        public string User { get; set; }

        public DateTime Started { get; set; }
        public DateTime? Ended { get; set; }
        public string Summary { get; set; }
        public string Log { get; set; }
        public string Exception { get; set; }

        public string Repository { get; set; }
        public string Project { get; set; }
        public string Version { get; set; }
    }

    #endregion

    #region ProjectMetadata
    public class MetadataEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class MetadataWrapper
    {
        private readonly IList<MetadataEntry> list;

        public MetadataWrapper(IList<MetadataEntry> list)
        {
            this.list = list;
        }

        public string this[string key]
        {
            get
            {
                var entry = list.FirstOrDefault(e => e.Key.Equals(key));

                if (entry == null)
                    return null;

                return entry.Value;
            }
            set
            {
                var entry = list.FirstOrDefault(p => p.Key.Equals(key));

                if (entry == null)
                {
                    entry = new MetadataEntry();
                    entry.Key = key;
                    list.Add(entry);
                }

                entry.Value = value;

                if (string.IsNullOrEmpty(entry.Value))
                    list.Remove(entry);
            }
        }
    }

    public class PackageCompilation
    {
        public string Mode { get; set; }

        public string Platform { get; set; }

        public string BinaryDistributionUrl { get; set; }

        public PackageImageFile[] ImageFiles { get; set; }
    }

    public class PackageImageFile
    {
        public string Name { get; set; }
    }

    public class PackageProject
    {
        public string Name { get; set; }

        public string Repository { get; set; }

        public string HomePageUrl { get; set; }

        public PackageVersion Version { get; set; }
    }

    public class PackageVersion
    {
        public string Name { get; set; }

        public string Project { get; set; }

        public string SourceDistributionUrl { get; set; }

        public PackageCompilation[] Compilations { get; set; }

        public List<MetadataEntry> Metadata { get; set; }
    }
    #endregion

    #region Plan

    public class Plan
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal MonthPrice { get; set; }
        public decimal YearPrice { get; set; }
        public int UserLimit { get; set; }
        public int PrivateRepoLimit { get; set; }
    }

    #endregion
}
