using System.IO;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.Core
{
    public interface IGatewayManager
    {        
        bool AuthenticateDownload(string company, string repository);
        bool AuthenticateUpload(string company, string repository);
        Version[] Index(Caller caller, string company, string repository);
        void Upload(Caller caller, Stream stream, string company, string repository);
        string Download(Caller caller, string company, string repository, string projectName, string versionName, string contentType);
        void Hide(Caller caller, string company, string repository, string projectName, string versionName);
        void Restore(Caller caller, string company, string repository, string projectName, string versionName);
    }
}