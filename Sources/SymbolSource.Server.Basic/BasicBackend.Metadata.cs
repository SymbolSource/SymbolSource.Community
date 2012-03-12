using System;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public ImageFile GetImageFile(string name, string symbolHash)
        {
            throw new NotImplementedException(this.configuration.DataPath);
        }

        public SourceFile[] GetSourceFileList(ref ImageFile imageFile)
        {
            throw new NotImplementedException();
        }

        public void CreateVersion(Version version)
        {
        }

        public Compilation[] GetCompilationList(ref Version version)
        {
            throw new NotImplementedException();
        }

        public void SetVersionHidden(ref Version version, bool hidden)
        {
            throw new NotImplementedException();
        }

        public Version[] GetVersions(ref Project project)
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