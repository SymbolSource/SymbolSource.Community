using System.IO;
using SymbolSource.Server.Management.Client.WebService;

namespace SymbolSource.Gateway.Core
{
    public interface IGatewayVersionExtractor
    {
        Version Extract(NuGet.IPackage package);
    }
}