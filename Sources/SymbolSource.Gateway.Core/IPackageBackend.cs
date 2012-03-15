using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.Core
{
    public interface IPackageBackend : IGatewayBackend
    {
        Caller CreateUserByKey(string company, string type, string value);
        string GetPackageLink(ref Version version, string contentType);
        Version[] GetPackages(ref Repository repository, string packageFormat);
        Version UploadPackage(PackageProject package, string packageFormat, byte[] packageData, byte[] symbolPackageData);
        void SetVersionHidden(ref Version version, bool hidden);
    }
}