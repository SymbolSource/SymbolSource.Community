using System.ComponentModel.Composition;
using NuGet;

namespace SymbolSource.Integration.NuGet.CommandLine
{
    [Export(typeof(IPackageRule))]
    public class SourcePackageRule : PackageRuleWrapper
    {
        public SourcePackageRule()
            : base(NuGet.SourcePackageRule.Validate)
        {
        }
    }
}