using System;

namespace SymbolSource.Integration.NuGet
{
    public static class PackageHelper
    {
        public static bool IsBinaryFile(IPackageFile file)
        {
            return file.Path.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)
                   || file.Path.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase)
                   || file.Path.EndsWith(".winmd", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsSymbolFile(IPackageFile file)
        {
            return file.Path.EndsWith(".pdb", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsSourceFile(IPackageFile file)
        {
            return file.Path.StartsWith(@"src\", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
