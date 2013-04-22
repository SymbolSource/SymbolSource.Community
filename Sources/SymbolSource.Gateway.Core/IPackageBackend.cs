using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.Core
{
    public interface IPackageBackend : IGatewayBackend
    {
        Caller CreateUserByKey(string company, string type, string value);

        void CreateProject(Project project);
        void SetProjectPermissions(User targetUser, Project project, Permission permission);

        string GetPackageLink(ref Version version, string contentType);
        Version[] GetPackages(ref Repository repository, ref PackageFilter filter, string packageFormat, string projectId);
        UploadReport UploadPackage(PackageProject package, string packageFormat, byte[] packageData, byte[] symbolPackageData);
        void SetVersionHidden(ref Version version, bool hidden);
    }
}