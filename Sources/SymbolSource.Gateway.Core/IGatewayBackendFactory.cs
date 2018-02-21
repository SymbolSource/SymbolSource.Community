using SymbolSource.Server.Management.Client.WebService;

namespace SymbolSource.Gateway.Core
{
    public interface IGatewayBackendFactory<out T> : IManagementPreauthentication where T : IGatewayBackend
    {
        T Create(Caller caller);
    }
}
