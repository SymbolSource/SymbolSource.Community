using System.IO;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.Core
{
    public interface IGatewayVersionExtractor
    {
        Version Extract(Stream stream);
    }
}