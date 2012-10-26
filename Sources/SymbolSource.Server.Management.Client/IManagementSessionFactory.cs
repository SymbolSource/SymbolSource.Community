using System.Security.Principal;

namespace SymbolSource.Server.Management.Client
{
    public interface IManagementSessionFactory : IManagementPreauthentication
    {
        IManagementSession Create(Caller caller);
        IManagementSession Create(IPrincipal caller);
    }
}
