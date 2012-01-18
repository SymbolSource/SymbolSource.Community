using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;

namespace SymbolSource.Integration.NuGet.PackageExplorer
{
    public static class PackageHelper
    {
        public static bool IsBinaryFile(IPackageFile file)
        {
            return file.Path.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)
                   || file.Path.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase);
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
