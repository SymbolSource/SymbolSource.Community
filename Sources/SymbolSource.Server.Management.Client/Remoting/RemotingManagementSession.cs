using System;
using System.Reflection;

namespace SymbolSource.Server.Management.Client.Remoting
{
    public class RemotingManagementSession : MarshalByRefObject, IManagementSession
    {
        protected readonly Caller caller;
        protected readonly User user;

        public RemotingManagementSession(Caller caller)
        {
            if (caller == null)
                throw new ArgumentNullException("caller");

            this.caller = caller;
            user = new User { Company = caller.Company, Name = caller.Name };
        }

        public void Dispose()
        {
        }

        public Caller Caller
        {
            get { return caller; }
        }

        public User User
        {
            get { return user; }
        }

        public virtual Caller CreateUserByKey(string company, string type, string value)
        {
            //TODO: tymczasowo dla bramki NuGet, aż nie wymyślimy co z tą metodą
            return new Caller { Company = company, KeyType = type, KeyValue = value };
        }

        public virtual Company[] GetCompanies()
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void CreateCompany(Company company)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void UpdateCompany(Company company)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void DeleteCompany(Company company)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual User[] GetUsers(ref Company company)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void CreateUser(User user, UserKey userKey)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void UpdateUser(User user)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void DeleteUser(User user)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Repository[] GetRepositories(ref Company company)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void CreateOrUpdateRepository(Repository repository)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void CreateRepository(Repository repository)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void UpdateRepository(Repository repository)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void DeleteRepository(Repository repository)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Project[] GetProjects(ref Repository repository)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void CreateOrUpdateProject(Project project)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void CreateProject(Project project)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void UpdateProject(Project project)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void DeleteProject(Project project)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Version[] GetVersions(ref Project project)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void CreateOrUpdateVersion(Version version)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void CreateVersion(Version version)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void UpdateVersion(Version version)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void DeleteVersion(Version version)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Version[] GetPackages(ref Repository repository, string packageFormat)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public UploadReport UploadPackage(PackageProject package, string packageFormat, byte[] packageData, byte[] symbolPackageData)
        {
            //TODO: quick and dirty
            try
            {
                var version = new Version {Company = caller.Company, Repository = package.Repository, Project = package.Name, Name = package.Version.Name};
                CreateVersion(version);
                PushPackage(ref version, packageData, package);
                CreateJob(symbolPackageData, package);
                return new UploadReport { Summary = "OK" };
            }
            catch(Exception e)
            {
                return new UploadReport
                           {
                               Summary = "Error",
                               Exception = e.ToString(),
                           };                
            }
        }

        //public virtual Version UploadPackage(PackageProject package, string packageFormat, byte[] packageData, byte[] symbolPackageData)
        //{
        //    throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        //}

        public virtual CompanyPermission[] GetCompanyPermissions(Company company)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual RepositoryPermission[] GetRepositoryPermissions(Repository repository)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual ProjectPermission[] GetProjectPermissions(Project project)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual VersionPermission[] GetVersionPermissions(Version version)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void SetCompanyPermissions(User targetUser, Company company, Permission permission)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void SetRepositoryPermissions(User targetUser, Repository repository, Permission permission)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void SetProjectPermissions(User targetUser, Project project, Permission permission)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void SetVersionPermissions(User targetUser, Version version, Permission permission)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual UserKey[] GetUserKeys()
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void AddUserKeys(UserKey[] keys)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void RemoveUserKeys(UserKey[] keys)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Version[] GetVersionLastList()
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Compilation[] GetCompilationList(ref Version version)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual ImageFile[] GetImageFileList(ref Compilation compilation)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual ImageFile[] GetImageFileListByReference(ref Reference reference)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual ImageFile GetImageFile(string name, string symbolHash)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual SourceFile[] GetSourceFileList(ref ImageFile imageFile)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Depedency[] GetDepedencyList(ref ImageFile imageFile)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual NodeImageFile[] GetNodeImageFiles(ref NodeImageFile nodeImageFile)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual string GetSourceFileLink(ref SourceFile sourceFile)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual string GetImageFileLink(ref ImageFile imageFile)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual string GetSymbolFileLink(ref ImageFile imageFile)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual string GetUserVisualStudioLink()
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Permissions GetUserPermissions(User targetUser)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Version SetVersionHidden(ref Version version, bool hidden)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void CreateJob(byte[] package, PackageProject project)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual UploadReport[] GetUploadReports()
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void PushPackage(ref Version version, byte[] package, PackageProject packageProject)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual string GetPackageLink(ref Version version, string contentType)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual Statistic[] GetStatistic(string[] names, StatisticPeriod period, DateTime? @from, DateTime? to)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual string[] GetAvailableStatisticNames()
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void LogImageFileFound(ImageFile imageFile)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void LogImageFileNotFound(string imageFileName, string symbolHash)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }

        public virtual void LogSourceFileFound(SourceFile sourceFile)
        {
            throw new NotImplementedException("Not implemented: " + MethodBase.GetCurrentMethod().Name);
        }
    }
}
