using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SymbolSource.Processing.Basic;

namespace SymbolSource.Integration.NuGet
{
    public static class SourcePackageRule
    {
        public static IEnumerable<PackageIssue> Validate(IPackage package)
        {
            var binaryStore = new BinaryStoreManager();
            var symbolStore = new SymbolStoreManager();

            var files = package.GetFiles().ToArray();

            var hasSymbols = files.Where(PackageHelper.IsSymbolFile).Any();
            var hasSources = files.Where(PackageHelper.IsSourceFile).Any();

            var hasName = package.FileName != null && package.FileName.EndsWith(".nupkg", StringComparison.CurrentCultureIgnoreCase);
            var hasSymbolsName = hasName && package.FileName.EndsWith(".symbols.nupkg", StringComparison.CurrentCultureIgnoreCase);

            var isSymbolPackage = hasSymbolsName || hasSymbols || hasSources;

            if (!hasName)
            {
                yield return UnableToVerifyPackageName(package.FileName);
            }
            else
            {
                if (!hasSymbolsName && (hasSymbols || hasSources))
                    yield return IncorrectSymbolPackageName(package.FileName);

                if (hasSymbolsName && !hasSymbols && !hasSources)
                    yield return IncorrectPackageName(package.FileName);
            }

            foreach (var binaryFile in files.Where(PackageHelper.IsBinaryFile))
            {
                string binaryHash;

                using (var stream = binaryFile.GetStream())
                    binaryHash = binaryStore.ReadPdbHash(stream);

                if (binaryHash == null)
                    yield return NoSymbolSupportIssue(binaryFile.Path);

                var symbolPath = Path.ChangeExtension(binaryFile.Path, ".pdb");
                var symbolFile = GetSingleFile(files, symbolPath);

                if (isSymbolPackage && symbolFile == null)
                    yield return MissingSymbolFileIssue(binaryFile.Path, symbolPath);

                if (!isSymbolPackage && symbolFile != null)
                    yield return UnnecessarySymbolFileIssue(binaryFile.Path, symbolFile.Path);

                if (symbolFile != null)
                {
                    string symbolHash;

                    using (var stream = symbolFile.GetStream())
                        symbolHash = symbolStore.ReadHash(stream);

                    if (binaryHash != symbolHash)
                        yield return HashMismatchIssue(binaryFile.Path, binaryHash, symbolFile.Path, symbolHash);
                }
            }

            foreach (var symbolFile in files.Where(PackageHelper.IsSymbolFile))
            {
                var paths = new[] { ".dll", ".exe", ".winmd" }
                    .Select(extension => Path.ChangeExtension(symbolFile.Path, extension))
                    .ToArray();

                if (paths.All(path => GetSingleFile(files, path) == null))
                    yield return OrphanSymbolFileIssue(symbolFile.Path, paths);
            }
        }

        private static IPackageFile GetSingleFile(IEnumerable<IPackageFile> files, string path)
        {
            return files.Where(file => file.Path.Equals(path, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
        }

        private static PackageIssue UnableToVerifyPackageName(string packageFileName)
        {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "Unable to verify package name",
                string.Format("The name of this package '{0}' cannot be verified. It is new or has been opened from a feed.", packageFileName),
                "Save the package to disk and try again.");
        }

        private static PackageIssue IncorrectSymbolPackageName(string packageFileName)
        {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "Incorrect symbol package name",
                string.Format("The name of this package '{0}' does not follow proper conventions. The name of a package containing symbols and/or sources should end with '.symbols.nupkg'.", packageFileName),
                "Rename this package file to end correctly or remove all symbols and sources it was not intended to be a symbol package.");
        }

        private static PackageIssue IncorrectPackageName(string packageFileName)
        {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "Incorrect package name",
                string.Format("The name of this package '{0}' does not follow proper conventions. It suggests that this is a symbol package, because it ends with '.symbols.nupkg'.", packageFileName),
                "Rename this package file to end correctly or add missing symbols and sources to make it a valid symbol package.");
        }

        private static PackageIssue NoSymbolSupportIssue(string binaryPath)
        {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "Assembly compiled without symbol support",
                string.Format("The assembly '{0}' was built with symbol generation disabled or symbol information has been later stripped off.", binaryPath),
                "Rebuild assembly with symbol generation turned on.");
        }

        private static PackageIssue MissingSymbolFileIssue(string binaryPath, string symbolPath)
        {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "Missing symbol file for assembly",
                string.Format("The assembly '{0}' does not have a corresponding symbol file '{1}'. All assemblies in a symbol package should have corresponding PDB files.", binaryPath, symbolPath),
                string.Format("Add symbol file '{0}' to the package.", symbolPath));
        }

        private static PackageIssue UnnecessarySymbolFileIssue(string binaryPath, string symbolPath)
        {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "Unnecessary symbol file for assembly",
                string.Format("The assembly '{0}' has a corresponding symbol file '{1}'. No PDB files should be present in a binary package as they disable symbol server support. ", binaryPath, symbolPath),
                string.Format("Remove symbol file '{0}' from the package.", symbolPath));
        }

        private static PackageIssue HashMismatchIssue(string binaryPath, string binaryHash, string symbolPath, string symbolHash)
        {
            return new PackageIssue(
                PackageIssueLevel.Error,
                "Mismatched assembly and symbol hashes",
                string.Format("The assembly '{0}' has a diffent hash ({1}) than its symbol file '{2}' ({3})", binaryPath, binaryHash, symbolPath, symbolHash),
                "Verify that correct files have been added to the package. Note that every compilation produces a different hash value, so both files must be taken from the same build.");
        }

        private static PackageIssue OrphanSymbolFileIssue(string symbolPath, IEnumerable<string> paths)
        {
            return new PackageIssue(
               PackageIssueLevel.Warning,
               "Orphan symbol file found",
               string.Format("The symbol file '{0}' does not have any corresponding assemblies ({1}).", symbolPath, string.Join(", ", paths.Select(path => "'" + path + "'"))),
               "Remove the symbol file or add the corresponding assembly.");
        }
    }
}