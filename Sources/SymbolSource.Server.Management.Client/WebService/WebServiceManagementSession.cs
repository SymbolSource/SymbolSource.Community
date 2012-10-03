using System;

namespace SymbolSource.Server.Management.Client
{
    public class WebServiceManagementSession : IManagementSession
    {
        private readonly WebService service;
        private readonly Caller caller;
        private User user;

        public WebServiceManagementSession(IWebServiceManagementConfiguration configuration, Caller caller)
        {
            service = new ConfigurableWebService(configuration);
            this.caller = caller;
        }

        public virtual void Dispose()
        {
            service.Dispose();
        }

        public Caller Caller
        {
            get { return caller; }
        }

        public User User
        {
            get
            {
                if(user == null)
                    user = service.UserValidate(caller);
                return user;
            }
        }

        public virtual Company[] GetCompanies()
        {
            return service.GetCompanies(caller);
        }

        public virtual void CreateCompany(Company company)
        {
            service.CreateCompany(caller, company);
        }

        public virtual void CreateCompany(User user, UserKey userKey, Plan plan)
        {
            service.CreateCompany2(caller, user, userKey, plan);
        }

        public virtual void UpdateCompany(Company company)
        {
            service.UpdateCompany(caller, company);
        }

        public virtual void DeleteCompany(Company company)
        {
            service.DeleteCompany(caller, company);
        }

        public virtual User[] GetUsers(ref Company company)
        {
            return service.GetUsers(caller, ref company);
        }

        public virtual void CreateUser(User user, UserKey userKey, Plan plan)
        {
            service.CreateUser(caller, user, userKey, plan);
        }

        public virtual void UpdateUser(User user)
        {
            service.UpdateUser(caller, ref user);
        }

        public virtual void DeleteUser(User user)
        {
            service.DeleteUser(caller, user);
        }

        public void SendPasswordMail(string url)
        {
            service.SendPasswordMail(caller, url);
        }

        public virtual void ResetPassword(UserKey userKey)
        {
            service.ResetPassword(caller, userKey);
        }

        public virtual Repository[] GetRepositories(ref Company company)
        {
            return service.GetRepositories(caller, ref company);
        }

        public virtual void CreateOrUpdateRepository(Repository repository)
        {
            service.CreateOrUpdateRepository(caller, ref repository);
        }

        public virtual void CreateRepository(Repository repository)
        {
            service.CreateRepository(caller, ref repository);
        }

        public virtual void UpdateRepository(Repository repository)
        {
            service.UpdateRepository(caller, ref repository);
        }

        public virtual void DeleteRepository(Repository repository)
        {
            service.DeleteRepository(caller, repository);
        }

        public virtual Project[] GetProjects(ref Repository repository)
        {
            return service.GetProjects(caller, ref repository);
        }

        public virtual void CreateOrUpdateProject(Project project)
        {
            service.CreateOrUpdateProject(caller, ref project);
        }

        public virtual void CreateProject(Project project)
        {
            service.CreateProject(caller, ref project);
        }

        public virtual void UpdateProject(Project project)
        {
            service.UpdateProject(caller, ref project);
        }

        public virtual void DeleteProject(Project project)
        {
            service.DeleteProject(caller, project);
        }

        public virtual Version[] GetVersions(ref Project project)
        {
            return service.GetVersions(caller, ref project);
        }

        public virtual void CreateOrUpdateVersion(Version version)
        {
            service.CreateOrUpdateVersion(caller, ref version);
        }

        public virtual void CreateVersion(Version version)
        {
            service.CreateVersion(caller, ref version);
        }

        public virtual void UpdateVersion(Version version)
        {
            service.UpdateVersion(caller, ref version);
        }

        public virtual void DeleteVersion(Version version)
        {
            service.DeleteVersion(caller, version);
        }

        public virtual Version[] GetPackages(ref Repository repository, ref PackageFilter filter, string packageFormat)
        {
            return service.GetPackages(caller, ref repository, ref filter, packageFormat);
        }

        public virtual UploadReport UploadPackage(PackageProject package, string packageFormat, byte[] packageData, byte[] symbolPackageData)
        {
            return service.UploadPackage(caller, package, packageFormat, packageData, symbolPackageData);
        }

        public virtual Version SetVersionHidden(ref Version version, bool hidden)
        {
            return service.SetVersionHidden(caller, ref version, hidden);
        }

        public virtual CompanyPermission[] GetCompanyPermissions(Company company)
        {
            return service.GetCompanyPermissions(caller, company);
        }

        public virtual RepositoryPermission[] GetRepositoryPermissions(Repository repository)
        {
            return service.GetRepositoryPermissions(caller, repository);
        }

        public virtual ProjectPermission[] GetProjectPermissions(Project project)
        {
            return service.GetProjectPermissions(caller, project);
        }

        public virtual VersionPermission[] GetVersionPermissions(Version version)
        {
            return service.GetVersionPermissions(caller, version);
        }

        public virtual void SetCompanyPermissions(User targetUser, Company company, Permission permission)
        {
            service.SetCompanyPermissions(caller, targetUser, company, permission);
        }

        public virtual void SetRepositoryPermissions(User targetUser, Repository repository, Permission permission)
        {
            service.SetRepositoryPermissions(caller, targetUser, repository, permission);
        }

        public virtual  void SetProjectPermissions(User targetUser, Project project, Permission permission)
        {
            service.SetProjectPermissions(caller, targetUser, project, permission);
        }

        public virtual void SetVersionPermissions(User targetUser, Version version, Permission permission)
        {
            service.SetVersionPermissions(caller, targetUser, version, permission);
        }

        public virtual Caller CreateUserByKey(string company, string type, string value)
        {
            //tu ma byc Create -> musze zmienic w webServisie
            return service.CreateUserByKey(company, type, value);
        }

        public virtual UserKey[] GetUserKeys(User targetUser)
        {
            return service.GetUserKeys(caller, targetUser);
        }

        public virtual void AddUserKey(User targetUser, UserKey key)
        {
            service.AddUserKey(caller, targetUser, key);
        }

        public virtual void RemoveUserKey(User targetUser, UserKey key)
        {
            service.RemoveUserKey(caller, targetUser, key);
        }

        public virtual Version[] GetVersionLastList()
        {
            return service.GetVersionLastList(caller);
        }

        public virtual Compilation[] GetCompilationList(ref Version version)
        {
            return service.GetCompilationList(caller, ref version);
        }

        public virtual ImageFile[] GetImageFileList(ref Compilation compilation)
        {
            return service.GetImageFileList(caller, ref compilation);
        }

        public virtual ImageFile[] GetImageFileListByReference(ref Reference reference)
        {
            return service.GetImageFileListByReference(caller, ref reference);
        }

        public virtual ImageFile GetImageFile(string name, string symbolHash)
        {
            return service.GetImageFile(caller, name, symbolHash);
        }

        public virtual SourceFile[] GetSourceFileList(ref ImageFile imageFile)
        {
            return service.GetSourceFileList(caller, ref imageFile);
        }

        public virtual Depedency[] GetDepedencyList(ref ImageFile imageFile)
        {
            return service.GetDepedencyList(caller, ref imageFile);
        }

        public virtual NodeImageFile[] GetNodeImageFiles(ref NodeImageFile nodeImageFile)
        {
            return service.GetNodeImageFiles(caller, ref nodeImageFile);
        }

        public virtual string GetSourceFileLink(ref SourceFile sourceFile)
        {
            return service.GetSourceFileLink(caller, ref sourceFile);
        }

        public virtual string GetImageFileLink(ref ImageFile imageFile)
        {
            return service.GetImageFileLink(caller, ref imageFile);
        }

        public virtual string GetSymbolFileLink(ref ImageFile imageFile)
        {
            return service.GetSymbolFileLink(caller, ref imageFile);
        }

        public virtual string GetUserVisualStudioLink(User targetUser)
        {
            return service.GetUserVisualStudioLink(caller, targetUser);
        }

        public virtual Permissions GetUserPermissions(User targetUser)
        {
            return service.GetUserPermissions(caller, targetUser);
        }

        public virtual UploadReport[] GetUploadReports()
        {
            return service.GetUploadReports(caller);
        }

        public virtual string GetPackageLink(ref Version version, string contentType)
        {
            return service.GetPackageLink(caller, ref version, contentType);
        }

        public virtual Statistic[] GetStatistic(string[] names, StatisticPeriod period, DateTime? from, DateTime? to)
        {
            return service.GetStatistic(caller, names, period, from, to);
        }

        public virtual string[] GetAvailableStatisticNames()
        {
            return service.GetAvailableStatisticNames(caller);
        }

        public virtual void LogImageFileFound(ImageFile imageFile)
        {
            service.LogImageFileFound(caller, imageFile);
        }

        public virtual void LogImageFileNotFound(string imageFileName, string symbolHash)
        {
            service.LogImageFileNotFound(caller, imageFileName, symbolHash);
        }

        public virtual void LogSourceFileFound(SourceFile sourceFile)
        {
            service.LogSourceFileFound(caller, sourceFile);
        }

        public virtual Plan[] GetPlansByType(string type)
        {
            return service.GetPlansByType(type);
        }

        public virtual void RemovePlan(string name, string type)
        {
            service.RemovePlan(name, type);
        }

        public virtual void CreatePlan(string name, string type, decimal monthPrice, decimal yearPrice, int userLimit, int privateRepoLimit)
        {
            service.CreatePlan(name, type, monthPrice, yearPrice, userLimit, privateRepoLimit);
        }
    }
}