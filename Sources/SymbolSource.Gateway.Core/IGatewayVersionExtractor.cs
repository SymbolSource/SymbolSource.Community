using System;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Gateway.Core
{
    public interface IGatewayVersionExtractor
    {
        Version Extract(String packagePath);
    }
}