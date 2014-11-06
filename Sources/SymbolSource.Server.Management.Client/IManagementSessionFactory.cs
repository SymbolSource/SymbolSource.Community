namespace SymbolSource.Server.Management.Client.WebService
{
    public interface IManagementSessionFactory : IManagementPreauthentication
    {
        IManagementSession Create(Caller caller);
    }
}
