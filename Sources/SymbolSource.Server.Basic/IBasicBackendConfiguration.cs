namespace SymbolSource.Server.Basic
{
    public interface IBasicBackendConfiguration
    {
        string DataPath { get; }
        string IndexPath { get; }
        string RemotePath { get; }
    }
}
