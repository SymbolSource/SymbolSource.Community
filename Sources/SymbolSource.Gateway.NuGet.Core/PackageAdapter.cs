using System;
using System.Linq;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Gateway.NuGet.Core
{
    public class PackageAdapter
    {
        private readonly Version version;
        private readonly Version[] packages;

        public PackageAdapter(Version version, Version[] packages)
        {
            this.version = version;
            this.packages = packages;
        }

        public string Id
        {
            get { return version.Project; }
        }

        public string Version
        {
            get { return version.Name; }
        }

        public string Title
        {
            get { return version.Project; }
        }

        public string Authors
        {
            get { return string.Empty; }
        }

        public string IconUrl 
        {
            get { return null; }
        }

        public string LicenseUrl 
        {
            get { return null; }
        }

        public string ProjectUrl 
        {
            get { return null; }
        }
       
        public int DownloadCount 
        {
            get { return -1; }
        }

        public bool RequireLicenseAcceptance 
        {
            get { return false; }
        }

        public string Description 
        {
            get { return null; }
        }

        public string Summary 
        {
            get { return null; }
        }

        public string ReleaseNotes 
        {
            get { return null; }
        }

        public DateTime Published 
        {
            get { return DateTime.Now; }
        }

        public DateTime LastUpdated 
        {
            get { return DateTime.Now; }
        }

        public string Dependencies 
        {
            get { return null; }
        }        

        public string PackageHash 
        {
            get { return version.PackageHash; }
        }

        public string PackageHashAlgorithm
        {
            get { return "SHA512"; }
        }

        public string PackageType 
        {
            get { return "Packages"; }
        }

        public long PackageSize 
        {
            get { return -1; }
        }

        public string Copyright 
        {
            get { return null; }
        }

        public string Tags 
        {
            get { return null; }
        }

        public bool IsAbsoluteLatestVersion
        {
            get { return IsLatestVersion; }
        }

        public bool IsLatestVersion
        {
            get
            {
                var latestVersion = packages
                    .Where(p => p.Project == version.Project)
                    .OrderByDescending(p => p.Name)
                    .FirstOrDefault();
                return latestVersion == version;
            }
        }

        public bool Listed
        {
            get { return true; }
        }
    }
}