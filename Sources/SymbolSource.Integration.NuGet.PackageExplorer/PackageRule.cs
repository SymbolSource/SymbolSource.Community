using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using NuGetPackageExplorer.Types;

namespace SymbolSource.Integration.NuGet.PackageExplorer
{
    public abstract class PackageRule : IPackageRule 
    {
        protected static bool IsBinaryFile(IPackageFile file)
        {
            return file.Path.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)
                   || file.Path.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase);
        }

        protected static bool IsSymbolFile(IPackageFile file)
        {
            return file.Path.EndsWith(".pdb", StringComparison.CurrentCultureIgnoreCase);
        }

        protected static bool IsSourceFile(IPackageFile file)
        {
            return file.Path.StartsWith(@"src\", StringComparison.CurrentCultureIgnoreCase);
        }

        protected IPackageFile GetSingleFile(IEnumerable<IPackageFile> files, string path)
        {
            return files.Where(file => file.Path.Equals(path, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();
        }

        public abstract IEnumerable<PackageIssue> Validate(IPackage package, string packageFileName);
    }
}