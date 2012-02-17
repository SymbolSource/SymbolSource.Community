using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.Core
{
    public interface IPackageBackend : IGatewayBackend
    {
        Caller CreateUserByKey(string company, string type, string value);
        string GetPackageLink(ref Version version, string contentType);
        Version[] GetPackages(ref Repository repository, string packageFormat);
        void CreateOrUpdateRepository(Repository repository);
        void CreateProject(Project project);
        void CreateOrUpdateProject(Project project);
        void SetProjectPermissions(User targetUser, Project project, Permission permission);
        void PushPackage(ref Version version, byte[] data, PackageProject metadata);
        void CreateJob(byte[] data, PackageProject metadata);
        Version[] GetVersions(ref Project project);
        void SetVersionHidden(ref Version version, bool hidden);

        //TODO: do wywalenia po wprowadzeniu sprawdzania zawartości paczek po stronie management
        Compilation[] GetCompilationList(ref Version version);
        void CreateVersion(Version version);
    }
}