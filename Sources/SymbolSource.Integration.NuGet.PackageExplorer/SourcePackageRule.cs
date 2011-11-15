using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NuGetPackageExplorer.Types;
using System.ComponentModel.Composition;
using NuGet;
using SymbolSource.Processing.Basic;
using SymbolSource.Processing.Basic.Projects;
using SymbolSource.Processing.Basic.Projects.FileInfos;
using DirectoryInfo = SymbolSource.Processing.Basic.Projects.FileInfos.DirectoryInfo;
using FileInfo = SymbolSource.Processing.Basic.Projects.FileInfos.FileInfo;

namespace SymbolSource.Integration.NuGet.PackageExplorer
{
    [Export(typeof(IPackageRule))]
    public class SymbolPackageRule : IPackageRule
    {
        public IEnumerable<PackageIssue> Validate(IPackage package, string packageFileName)
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

    public class PackageDirectoryInfo : DirectoryInfo
    {
        private readonly IEnumerable<IPackageFile> files;
        private readonly string name;

        private PackageDirectoryInfo(IEnumerable<IPackageFile> files, DirectoryInfoFactory directoryInfoFactory, DirectoryInfo parentInfo, string name)
            : base(directoryInfoFactory, parentInfo)
        {
            this.files = files;
            this.name = name;
        }


        public PackageDirectoryInfo(IEnumerable<IPackageFile> files)
            : this(files, new DirectoryInfoFactory(), null, "")
        {
        }

        public override string Name
        {
            get { return name; }
        }

        protected override IEnumerable<IDirectoryInfo> ExecuteGetDirectories()
        {
            return files
                 .Where(file => file.Path.StartsWith(FullPath))
                 .Select(file => file.Path.Substring(FullPath.Length).Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
                 .Where(path => path.Length > 1)
                 .Select(path => path[0])
                 .Distinct()
                 .Select(name => new PackageDirectoryInfo(files, DirectoryInfoFactory, this, name));
        }

        protected override IEnumerable<IFileInfo> ExecuteGetFiles()
        {
            return files
                .Where(file => file.Path.StartsWith(FullPath))
                .Where(file => file.Path.Substring(FullPath.Length).Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                .Select(file => new PackageFileInfo(this, file));
        }
    }

    public class PackageFileInfo : FileInfo
    {
        private readonly IPackageFile file;

        public PackageFileInfo(DirectoryInfo parentInfo, IPackageFile file)
            : base(parentInfo)
        {
            this.file = file;
        }

        public override string Name
        {
            get { return file.Path.Split(Path.DirectorySeparatorChar).Last(); }
        }

        public override Stream GetStream(FileMode fileMode)
        {
            return file.GetStream();
        }
    }
}