namespace SymbolSource.Server.Management.Client
{
    public interface IWebServiceManagementConfiguration
    {
        string ManagementProxyPath { get; }
        bool IsRedirected { get; }
    }
}