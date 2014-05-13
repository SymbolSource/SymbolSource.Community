using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using NuGetPackageExplorer.Types;

namespace SymbolSource.Integration.NuGet.PackageExplorer
{
    public class PackageRuleWrapper : IPackageRule
    {
        private readonly Func<IPackage, IEnumerable<PackageIssue>> rule;

        public PackageRuleWrapper(Func<IPackage, IEnumerable<PackageIssue>> rule)
        {
            this.rule = rule;
        }

        public IEnumerable<NuGetPackageExplorer.Types.PackageIssue> Validate(global::NuGet.IPackage package, string packageFileName)
        {
            return rule(new PackageWrapper(package, packageFileName)).Select(pi => pi.ToCommandLine());
        }
    }

    public class PackageWrapper : IPackage
    {
        private readonly global::NuGet.IPackage package;
        private readonly string packageFileName;

        public PackageWrapper(global::NuGet.IPackage package, string packageFileName)
        {
            this.package = package;
            this.packageFileName = packageFileName;
        }

        public string FileName
        {
            get { return packageFileName; }
        }

        public IEnumerable<IPackageFile> GetFiles()
        {
            return package.GetFiles().Select(p => new PackageFileWrapper(p));
        }
    }

    public class PackageFileWrapper : IPackageFile
    {
        private readonly global::NuGet.IPackageFile file;

        public PackageFileWrapper(global::NuGet.IPackageFile file)
        {
            this.file = file;
        }

        public string Path
        {
            get { return file.Path; }
        }

        public Stream GetStream()
        {
            return file.GetStream();
        }
    }

    public static class PackageIssueExtensions
    {
        public static NuGetPackageExplorer.Types.PackageIssue ToCommandLine(this PackageIssue issue)
        {
            return new NuGetPackageExplorer.Types.PackageIssue(issue.Level.ToCommandLine(), issue.Title, issue.Description, issue.Solution);
        }
    }

    public static class PackageIssueLevelExtensions
    {
        public static NuGetPackageExplorer.Types.PackageIssueLevel ToCommandLine(this PackageIssueLevel level)
        {
            switch (level)
            {
                case PackageIssueLevel.Error:
                    return NuGetPackageExplorer.Types.PackageIssueLevel.Error;
                case PackageIssueLevel.Warning:
                    return NuGetPackageExplorer.Types.PackageIssueLevel.Warning;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }
    }
}
