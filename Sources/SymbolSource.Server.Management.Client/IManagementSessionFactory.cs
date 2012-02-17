namespace SymbolSource.Server.Management.Client
{
    public interface IManagementSessionFactory : IManagementPreauthentication
    {
        IManagementSession Create(Caller caller);
    }
}
