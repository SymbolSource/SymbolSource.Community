using System.ComponentModel.Composition;
using NuGet;

namespace SymbolSource.Integration.NuGet.CommandLine
{
    [Export(typeof(IPackageRule))]
    public class SymbolPackageRule : PackageRuleWrapper
    {
        public SymbolPackageRule()
            : base(NuGet.SymbolPackageRule.Validate)
        {
        }
    }
}