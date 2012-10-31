using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using NuGet;

namespace SymbolSource.Integration.NuGet.CommandLine
{
    public class PackageRuleWrapper : IPackageRule
    {
        private readonly Func<IPackage, IEnumerable<PackageIssue>> rule;

        public PackageRuleWrapper(Func<IPackage, IEnumerable<PackageIssue>> rule)
        {
            this.rule = rule;
        }

        public IEnumerable<global::NuGet.PackageIssue> Validate(global::NuGet.IPackage package)
        {
            return rule(new PackageWrapper(package)).Select(pi => pi.ToCommandLine());
        }
    }

    public class PackageWrapper : IPackage
    {
        private readonly global::NuGet.IPackage package;

        public PackageWrapper(global::NuGet.IPackage package)
        {
            this.package = package;
        }

        public string FileName
        {
            get { return null; }
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
        public static global::NuGet.PackageIssue ToCommandLine(this PackageIssue issue)
        {
            return new global::NuGet.PackageIssue(issue.Title, issue.Description, issue.Solution, issue.Level.ToCommandLine());
        }
    }

    public static class PackageIssueLevelExtensions
    {
        public static global::NuGet.PackageIssueLevel ToCommandLine(this PackageIssueLevel level)
        {
            switch (level)
            {
                case PackageIssueLevel.Error:
                    return global::NuGet.PackageIssueLevel.Error;
                case PackageIssueLevel.Warning:
                    return global::NuGet.PackageIssueLevel.Warning;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }
    }
}
