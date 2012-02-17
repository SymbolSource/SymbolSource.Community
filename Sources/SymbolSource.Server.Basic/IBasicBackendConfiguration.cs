namespace SymbolSource.Server.Basic
{
    public interface IBasicBackendConfiguration
    {
        string LocalPath { get; }
        string RemotePath { get; }
    }
}
