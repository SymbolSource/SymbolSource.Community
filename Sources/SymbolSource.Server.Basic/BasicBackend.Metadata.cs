using System;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public ImageFile GetImageFile(string name, string symbolHash)
        {
            throw new NotImplementedException();
        }

        public SourceFile[] GetSourceFileList(ref ImageFile imageFile)
        {
            throw new NotImplementedException();
        }

        public void CreateVersion(Version version)
        {
            throw new NotImplementedException();
        }

        public Compilation[] GetCompilationList(ref Server.Management.Client.Version version)
        {
            throw new NotImplementedException();
        }

        public void SetVersionHidden(ref Server.Management.Client.Version version, bool hidden)
        {
            throw new NotImplementedException();
        }

        public Server.Management.Client.Version[] GetVersions(ref Project project)
        {
            throw new NotImplementedException();
        }

        public void SetProjectPermissions(User targetUser, Project project, Permission permission)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdateProject(Project project)
        {
            throw new NotImplementedException();
        }

        public void CreateProject(Project project)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdateRepository(Repository repository)
        {
            throw new NotImplementedException();
        }

        public Version[] GetPackages(ref Repository repository, string packageFormat)
        {
            throw new NotImplementedException();
        }

        public Caller CreateUserByKey(string company, string type, string value)
        {
            throw new NotImplementedException();
        }
    }
}