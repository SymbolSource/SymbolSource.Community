using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SymbolSource.Processing.Basic;
using SymbolSource.Processing.Basic.Projects;

namespace SymbolSource.Integration.NuGet
{
    public static class SymbolPackageRule
    {
        public static IEnumerable<PackageIssue> Validate(IPackage package)
        {
            var builder = new AddInfoBuilder(new BinaryStoreManager(), new SymbolStoreManager(), new SourceDiscover(new ManagedSourceExtractor(), new SourceStoreManager()));
            var info = builder.Build(new PackageDirectoryInfo(package.GetFiles().ToArray()));

            foreach (var binaryInfo in info.Binaries)
                if (binaryInfo.SymbolInfo != null && binaryInfo.SymbolInfo.SourceInfos != null)
                    foreach (var sourceInfo in binaryInfo.SymbolInfo.SourceInfos)
                        if (sourceInfo.ActualPath == null)
                            yield return NoSourceFileIssue(binaryInfo.File.FullPath, sourceInfo.OriginalPath);

            //var matched = info.Binaries
            //    .SelectMany(binary => binary.SymbolInfo.SourceInfos)
            //    .Where(source => source.ActualPath != null)
            //    .Select(source => source.ActualPath.FullPath)
            //    .ToList();
            
            //foreach (var file in package.GetFilesInFolder("src"))
            //{
            //    if (!matched.Contains(file))
            //        yield return UnnecessarySourceFileIssue(file);
            //}
        }

        private static PackageIssue NoSourceFileIssue(string binaryPath, string sourcePath)
        {
            return new PackageIssue(
                PackageIssueLevel.Error,
                "Missing source file",
                string.Format("The assembly '{0}' was built form a source file that cannot be located - '{1}'.", binaryPath, sourcePath),
                "Verify that the 'src' folder maintains proper relative paths of all sources and add the missing file if needed.");
        }

        private static PackageIssue UnnecessarySourceFileIssue(string sourcePath)
        {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "Unnecessary source file",
                string.Format("The source file '{0}' is not referenced by any of the packaged assemblies.", sourcePath),
                "Verify that the 'src' folder only contains source files used to build assemblies placed in the 'lib' folder.");
        }
    }

    public class PackageDirectoryInfo : Processing.Basic.Projects.IPackageFile
    {
        private readonly IEnumerable<IPackageFile> files;

        public PackageDirectoryInfo(IEnumerable<IPackageFile> files)
        {
            this.files = files;
        }

        public IEnumerable<IPackageEntry> Entries
        {
            get { return files.Select(f => new PackageFileInfo(f)); }
        }
    }

    public class PackageFileInfo : IPackageEntry
    {
        private readonly IPackageFile file;

        public PackageFileInfo(IPackageFile file)
        {
            this.file = file;
        }

        public string FullPath
        {
            get { return file.Path; }
        }

        public Stream Stream
        {
            get { return file.GetStream(); }
        }

        public DateTime CreationTimeUtc
        {
            get { throw new NotImplementedException(); }
        }
    }
}