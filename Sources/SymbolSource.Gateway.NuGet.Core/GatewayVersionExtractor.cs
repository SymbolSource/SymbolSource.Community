using System;
using SymbolSource.Gateway.Core;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Gateway.NuGet.Core
{
    public interface INuGetGatewayVersionExtractor : IGatewayVersionExtractor
    {

    }

    public class NuGetGatewayVersionExtractor : INuGetGatewayVersionExtractor
    {
        public Version Extract(String packagePath)
        {
            var version = NuGetTranslator.ConvertToVersion(packagePath);
            return version;
        }
    }
}