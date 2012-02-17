using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.Core
{
    public interface IGatewayBackendFactory<out T> : IManagementPreauthentication where T : IGatewayBackend
    {
        T Create(Caller caller);
    }
}
