using NuGetPackageExplorer.Types;

namespace SymbolSource.Integration.NuGet.PackageExplorer
{
    [PackageCommandMetadata("Symbol server status...")]
    public class SymbolServerStatusCommand : IPackageCommand
    {
        public void Execute(global::NuGet.IPackage package, string packagePath)
        {
            new Form1(new SymbolServerChecker(new PackageWrapper(package, packagePath))).ShowDialog();
        }
    }
}
