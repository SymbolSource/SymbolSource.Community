using System.IO;
using NuGet;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client.WebService;

namespace SymbolSource.Gateway.NuGet.Core
{
    public interface INuGetGatewayVersionExtractor : IGatewayVersionExtractor
    {
        
    }

    public class NuGetGatewayVersionExtractor : INuGetGatewayVersionExtractor
    {
        public Version Extract(IPackage package)
        {
            return NuGetTranslator.ConvertToVersion(package);
        }
    }
}