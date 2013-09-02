using System.IO;
using NuGet;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.NuGet.Core
{
    public interface INuGetGatewayVersionExtractor : IGatewayVersionExtractor
    {
        
    }

    public class NuGetGatewayVersionExtractor : INuGetGatewayVersionExtractor
    {
        public Version Extract(string path)
        {
            return NuGetTranslator.ConvertToVersion(path);            
        }
    }
}