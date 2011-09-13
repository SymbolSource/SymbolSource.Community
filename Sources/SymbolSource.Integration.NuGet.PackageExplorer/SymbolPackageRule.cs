using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NuGetPackageExplorer.Types;
using System.ComponentModel.Composition;
using NuGet;
using SymbolSource.Processing.Basic;

namespace SymbolSource.Integration.NuGet.PackageExplorer 
{
    [Export(typeof(IPackageRule))]
    public class SourcePackageRule : PackageRule
    {
        public override IEnumerable<PackageIssue> Validate(IPackage package)
        {
            var binaryStore = new BinaryStoreManager();
            var symbolStore = new SymbolStoreManager();

            var files = package.GetFiles().ToArray();
            
            //TODO: need a better way to determine if we have a symbols package
            var isSymbol = package.GetFilesInFolder("src").Any();

            foreach (var binaryFile in files.Where(IsBinaryFile))
            {
                string binaryHash;
                
                using (var stream = binaryFile.GetStream())
                    binaryHash = binaryStore.ReadPdbHash(stream);

                if (binaryHash == null)
                    yield return NoSymbolSupportIssue(binaryFile.Path);

                var symbolPath = Path.ChangeExtension(binaryFile.Path, ".pdb");
                var symbolFile = GetSingleFile(files, symbolPath);

                if (isSymbol && symbolFile == null)
                    yield return MissingSymbolFileIssue(binaryFile.Path, symbolPath);

                if (!isSymbol && symbolFile != null)
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

            foreach (var symbolFile in files.Where(IsSymbolFile))
            {
                var dllPath = Path.ChangeExtension(symbolFile.Path, ".dll");
                var exePath = Path.ChangeExtension(symbolFile.Path, ".exe");

                if (GetSingleFile(files, dllPath) == null && GetSingleFile(files, exePath) == null)
                    yield return OrphanSymbolFileIssue(symbolFile.Path, dllPath, exePath);
            }
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

        private static PackageIssue OrphanSymbolFileIssue(string symbolPath, string dllPath, string exePath)
        {
            return new PackageIssue(
               PackageIssueLevel.Warning,
               "Orphan symbol file found",
               string.Format("The symbol file '{0}' does not have any corresponding assemblies ('{1}', '{2}').", symbolPath, dllPath, exePath),
               "Remove the symbol file or add the corresponding assembly.");
        }
    }
}