using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

namespace SymbolSource.Server.Management.Client
{
    public class WebServiceManagementSession : IManagementSession
    {
        private readonly IWebService service;
        private readonly Caller caller;
        private User user;

        public WebServiceManagementSession(IWebServiceManagementConfiguration configuration, Caller caller)
        {
            var configurableService = new ConfigurableWebService(configuration);     
            configurableService.ClientCredentials.UserName.UserName = caller.Company + "\\" + caller.Name;
            configurableService.ClientCredentials.UserName.Password = caller.KeyValue;
            this.caller = caller;
            service = configurableService;
        }

        public WebServiceManagementSession(IWebServiceManagementConfiguration configuration, IPrincipal principal)
        {
            var configurableService = new ConfigurableWebService(configuration);
            configurableService.ClientCredentials.UserName.UserName = "Anonymous";

            var claims = principal as ClaimsPrincipal;

            if(claims == null)
                throw new Exception();

            var context = claims.Identities.First().BootstrapContext;
            if (context == null)
                throw new Exception();

            service = configurableService.ChannelFactory.CreateChannelWithActAsToken(((BootstrapContext) context).SecurityToken);

            caller = new Caller
                         {
                             Company = claims.FindFirst("http://schemas.symbolsource.org/claims/company").Value, 
                             Name = claims.FindFirst(ClaimTypes.Name).Value
                         };
        }

        public virtual void Dispose()
        {
            //service.Dispose();
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
            return service.GetCompanies();
        }

        public virtual void CreateCompany(Company company)
        {
            service.CreateCompany(company);
        }

        public virtual void CreateCompany(User user, UserKey userKey)
        {
            service.CreateCompany2(user, userKey);
        }

        public virtual void UpdateCompany(Company company)
        {
            service.UpdateCompany(company);
        }

        public virtual void DeleteCompany(Company company)
        {
            service.DeleteCompany(company);
        }

        public virtual User[] GetUsers(ref Company company)
        {
            return service.GetUsers(ref company);
        }

        public virtual void CreateUser(User user, UserKey userKey)
        {
            service.CreateUser(user, userKey);
        }

        public virtual void UpdateUser(User user)
        {
            service.UpdateUser(ref user);
        }

        public virtual void DeleteUser(User user)
        {
            service.DeleteUser(user);
        }

        public void SendPasswordMail(Caller caller, string url)
        {
            service.SendPasswordMail(caller, url);
        }

        public virtual void ResetPassword(UserKey userKey)
        {
            service.ResetPassword(caller, userKey);
        }

        public virtual Repository[] GetRepositories(ref Company company)
        {
            return service.GetRepositories(ref company);
        }

        public virtual void CreateOrUpdateRepository(Repository repository)
        {
            service.CreateOrUpdateRepository(ref repository);
        }

        public virtual void CreateRepository(Repository repository)
        {
            service.CreateRepository(ref repository);
        }

        public virtual void UpdateRepository(Repository repository)
        {
            service.UpdateRepository(ref repository);
        }

        public virtual void DeleteRepository(Repository repository)
        {
            service.DeleteRepository(repository);
        }

        public virtual Project[] GetProjects(ref Repository repository)
        {
            return service.GetProjects(ref repository);
        }

        public virtual void CreateOrUpdateProject(Project project)
        {
            service.CreateOrUpdateProject(ref project);
        }

        public virtual void CreateProject(Project project)
        {
            service.CreateProject(ref project);
        }

        public virtual void UpdateProject(Project project)
        {
            service.UpdateProject(ref project);
        }

        public virtual void DeleteProject(Project project)
        {
            service.DeleteProject(project);
        }

        public virtual Version[] GetVersions(ref Project project)
        {
            return service.GetVersions(ref project);
        }

        public virtual void CreateOrUpdateVersion(Version version)
        {
            service.CreateOrUpdateVersion(ref version);
        }

        public virtual void CreateVersion(Version version)
        {
            service.CreateVersion(ref version);
        }

        public virtual void UpdateVersion(Version version)
        {
            service.UpdateVersion(ref version);
        }

        public virtual void DeleteVersion(Version version)
        {
            service.DeleteVersion(version);
        }

        public virtual Version[] GetPackages(ref Repository repository, ref PackageFilter filter, string packageFormat)
        {
            return service.GetPackages(ref repository, ref filter, packageFormat);
        }

        public virtual UploadReport UploadPackage(PackageProject package, string packageFormat, byte[] packageData, byte[] symbolPackageData)
        {
            return service.UploadPackage(package, packageFormat, packageData, symbolPackageData);
        }

        public virtual Version SetVersionHidden(ref Version version, bool hidden)
        {
            return service.SetVersionHidden(ref version, hidden);
        }

        public virtual CompanyPermission[] GetCompanyPermissions(Company company)
        {
            return service.GetCompanyPermissions(company);
        }

        public virtual RepositoryPermission[] GetRepositoryPermissions(Repository repository)
        {
            return service.GetRepositoryPermissions(repository);
        }

        public virtual ProjectPermission[] GetProjectPermissions(Project project)
        {
            return service.GetProjectPermissions(project);
        }

        public virtual VersionPermission[] GetVersionPermissions(Version version)
        {
            return service.GetVersionPermissions(version);
        }

        public virtual void SetCompanyPermissions(User targetUser, Company company, Permission permission)
        {
            service.SetCompanyPermissions(targetUser, company, permission);
        }

        public virtual void SetRepositoryPermissions(User targetUser, Repository repository, Permission permission)
        {
            service.SetRepositoryPermissions(targetUser, repository, permission);
        }

        public virtual  void SetProjectPermissions(User targetUser, Project project, Permission permission)
        {
            service.SetProjectPermissions(targetUser, project, permission);
        }

        public virtual void SetVersionPermissions(User targetUser, Version version, Permission permission)
        {
            service.SetVersionPermissions(targetUser, version, permission);
        }

        public virtual Caller CreateUserByKey(string company, string type, string value)
        {
            //tu ma byc Create -> musze zmienic w webServisie
            return service.CreateUserByKey(company, type, value);
        }

        public virtual UserKey[] GetUserKeys(User targetUser)
        {
            return service.GetUserKeys(targetUser);
        }

        public virtual void AddUserKey(User targetUser, UserKey key)
        {
            service.AddUserKey(targetUser, key);
        }

        public virtual void RemoveUserKey(User targetUser, UserKey key)
        {
            service.RemoveUserKey(targetUser, key);
        }

        public virtual Version[] GetVersionLastList()
        {
            return service.GetVersionLastList();
        }

        public virtual Compilation[] GetCompilationList(ref Version version)
        {
            return service.GetCompilationList(ref version);
        }

        public virtual ImageFile[] GetImageFileList(ref Compilation compilation)
        {
            return service.GetImageFileList(ref compilation);
        }

        public virtual ImageFile[] GetImageFileListByReference(ref Reference reference)
        {
            return service.GetImageFileListByReference(ref reference);
        }

        public virtual ImageFile GetImageFile(string name, string symbolHash)
        {
            return service.GetImageFile(name, symbolHash);
        }

        public virtual SourceFile[] GetSourceFileList(ref ImageFile imageFile)
        {
            return service.GetSourceFileList(ref imageFile);
        }

        public virtual Depedency[] GetDepedencyList(ref ImageFile imageFile)
        {
            return service.GetDepedencyList(ref imageFile);
        }

        public virtual NodeImageFile[] GetNodeImageFiles(ref NodeImageFile nodeImageFile)
        {
            return service.GetNodeImageFiles(ref nodeImageFile);
        }

        public virtual string GetSourceFileLink(ref SourceFile sourceFile)
        {
            return service.GetSourceFileLink(ref sourceFile);
        }

        public virtual string GetImageFileLink(ref ImageFile imageFile)
        {
            return service.GetImageFileLink(ref imageFile);
        }

        public virtual string GetSymbolFileLink(ref ImageFile imageFile)
        {
            return service.GetSymbolFileLink(ref imageFile);
        }

        public virtual string GetUserVisualStudioLink(User targetUser)
        {
            return service.GetUserVisualStudioLink(targetUser);
        }

        public virtual Permissions GetUserPermissions(User targetUser)
        {
            return service.GetUserPermissions(targetUser);
        }

        public virtual UploadReport[] GetUploadReports()
        {
            return service.GetUploadReports();
        }

        public virtual string GetPackageLink(ref Version version, string contentType)
        {
            return service.GetPackageLink(ref version, contentType);
        }

        public virtual Statistic[] GetStatistic(string[] names, StatisticPeriod period, DateTime? from, DateTime? to)
        {
            return service.GetStatistic(names, period, from, to);
        }

        public virtual string[] GetAvailableStatisticNames()
        {
            return service.GetAvailableStatisticNames();
        }

        public virtual void LogImageFileFound(ImageFile imageFile)
        {
            service.LogImageFileFound(imageFile);
        }

        public virtual void LogImageFileNotFound(string imageFileName, string symbolHash)
        {
            service.LogImageFileNotFound(imageFileName, symbolHash);
        }

        public virtual void LogSourceFileFound(SourceFile sourceFile)
        {
            service.LogSourceFileFound(sourceFile);
        }

        public string PaymentPrepare(string plan, string returnUrl, string cancelUrl)
        {
            return service.PaymentPrepare(plan, returnUrl, cancelUrl);
        }

        public void PaymentDoAction(string plan, string token)
        {
            service.PaymentDoAction(plan, token);
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