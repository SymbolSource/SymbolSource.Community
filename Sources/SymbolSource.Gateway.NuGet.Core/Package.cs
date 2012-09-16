using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Routing;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using NuGet;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Gateway.NuGet.Core
{
    [DataServiceKey("Id", "Version")]
    [EntityPropertyMapping("Id", SyndicationItemProperty.Title, SyndicationTextContentKind.Plaintext, keepInContent: false)]
    [EntityPropertyMapping("Authors", SyndicationItemProperty.AuthorName, SyndicationTextContentKind.Plaintext, keepInContent: false)]
    [EntityPropertyMapping("LastUpdated", SyndicationItemProperty.Updated, SyndicationTextContentKind.Plaintext, keepInContent: false)]
    [EntityPropertyMapping("Summary", SyndicationItemProperty.Summary, SyndicationTextContentKind.Plaintext, keepInContent: false)]
    [HasStream]
    public class Package
    {
        public Package(Version version, Version[] packages)
        {
            Id = version.Project;
            Version = version.Name;
            Title = version.Project;
            PackageHash = version.PackageHash;
            PackageHashAlgorithm = "SHA512";
            PackageSize = -1;
            LastUpdated = DateTime.UtcNow;
            Published = DateTime.UtcNow;
            IsLatestVersion = packages.Where(p => p.Project == version.Project).OrderByDescending(p => p.Name).FirstOrDefault() == version;
            IsAbsoluteLatestVersion = IsLatestVersion;
            //Path = derivedData.Path;
            //FullPath = derivedData.FullPath;
        }

        internal string FullPath
        {
            get;
            set;
        }

        internal string Path
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Authors
        {
            get;
            set;
        }

        public string IconUrl
        {
            get;
            set;
        }

        public string LicenseUrl
        {
            get;
            set;
        }

        public string ProjectUrl
        {
            get;
            set;
        }

        public int DownloadCount
        {
            get;
            set;
        }

        public bool RequireLicenseAcceptance
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Summary
        {
            get;
            set;
        }

        public string ReleaseNotes
        {
            get;
            set;
        }

        public DateTime Published
        {
            get;
            set;
        }

        public DateTime LastUpdated
        {
            get;
            set;
        }

        public string Dependencies
        {
            get;
            set;
        }

        public string PackageHash
        {
            get;
            set;
        }

        public string PackageHashAlgorithm
        {
            get;
            set;
        }

        public long PackageSize
        {
            get;
            set;
        }

        public string Copyright
        {
            get;
            set;
        }

        public string Tags
        {
            get;
            set;
        }

        public bool IsAbsoluteLatestVersion
        {
            get;
            set;
        }

        public bool IsLatestVersion
        {
            get;
            set;
        }

        public bool Listed
        {
            get;
            set;
        }

        public int VersionDownloadCount
        {
            get;
            set;
        }
    }

    public class PackageContext
    {
        private readonly IPackageBackend backend;
        private readonly Repository repository;

        public PackageContext(IPackageBackend backend, Repository repository)
        {
            this.backend = backend;
            this.repository = repository;
        }

        public IQueryable<Package> Packages
        {
            get
            {
                var repository = this.repository;
                var versions = backend.GetPackages(ref repository, "NuGet");
                return versions.Select(v => new Package(v, versions)).AsQueryable();
            }
        }
    }

    [RewriteBaseUrlBehavior]
    public class Packages : DataService<PackageContext>, IDataServiceStreamProvider, IServiceProvider
    {
        public static void MapRoutes(RouteCollection routes, string prefix, string suffix)
        {
            var factory = new DataServiceHostFactory();
            string servicePrefix = prefix + "/{repository}/FeedService.mvc2";
            RouteTable.Routes.Add(new DynamicServiceRoute(servicePrefix, null, factory, typeof(Packages)));
        }


        private IGatewayBackendFactory<IPackageBackend> BackendFactory
        {
            get { return ServiceLocator.Resolve<IGatewayBackendFactory<IPackageBackend>>(); }
        }

        private INuGetGatewayManager Manager
        {
            get { return ServiceLocator.Resolve<INuGetGatewayManager>(); }
        }

        private IPackageBackend Backend
        {
            get
            {                
                return BackendFactory.Create(Caller);
            }
        }

        private Caller Caller
        {
            get { return (Caller)HttpContext.Current.Items["Caller"]; }
        }

        private Repository Repository
        {
            get { return (Repository) HttpContext.Current.Items["Repository"]; }
        }

        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("Packages", EntitySetRights.AllRead);
            config.SetEntitySetPageSize("Packages", 100);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
            config.UseVerboseErrors = true;
            RegisterServices(config);
        }

        internal static void RegisterServices(IDataServiceConfiguration config)
        {
            config.SetServiceOperationAccessRule("Search", ServiceOperationRights.AllRead);
            config.SetServiceOperationAccessRule("FindPackagesById", ServiceOperationRights.AllRead);
            config.SetServiceOperationAccessRule("GetUpdates", ServiceOperationRights.AllRead);
        }

        protected override PackageContext CreateDataSource()
        {
            return new PackageContext(Backend, Repository);
        }

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            throw new NotSupportedException();
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            var package = (Package)entity;
            string url = Manager.Download(Caller, Repository.Company, Repository.Name, package.Id, package.Version, "application/zip");
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            return response.GetResponseStream();
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            return null;
            var package = (Package)entity;
            //manager.Download(caller, company, repository, project, version, contentType)
            // the URI need to ends with a '/' to be correctly merged so we add it to the application if it 
            string downloadUrl = "/onet.pl";//PackageUtility.GetPackageDownloadUrl(package);
            return new Uri(operationContext.AbsoluteServiceUri, "Download/123/123");
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            return "application/zip";
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            return null;
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            throw new NotSupportedException();
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            throw new NotSupportedException();
        }

        public int StreamBufferSize
        {
            get
            {
                return 64000;
            }
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceStreamProvider))
            {
                return this;
            }
            return null;
        }

        [WebGet]
        public IQueryable<Package> Search(string searchTerm, string targetFramework, bool includePrerelease)
        {
            IEnumerable<string> targetFrameworks = String.IsNullOrEmpty(targetFramework) ? Enumerable.Empty<string>() : targetFramework.Split('|');

            var repository = Repository;
            var versions = Backend.GetPackages(ref repository, "NuGet");
            var packages = versions.Select(v => new Package(v, versions)).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim('\'');
                packages = packages.Where(p => p.Title.Contains(searchTerm) || p.Version.Contains(searchTerm));
            }

            return packages;
        }

        [WebGet]
        public IQueryable<Package> FindPackagesById(string id)
        {
            var repository = Repository;
            var versions = Backend.GetPackages(ref repository, "NuGet");
            var packages = versions.Select(v => new Package(v, versions)).AsQueryable();

            packages = packages.Where(p => p.Id == id);
            return packages;
        }

        [WebGet]
        public IQueryable<Package> GetUpdates(string packageIds, string versions, bool includePrerelease, bool includeAllVersions, string targetFrameworks)
        {
            if (String.IsNullOrEmpty(packageIds) || String.IsNullOrEmpty(versions))
            {
                return Enumerable.Empty<Package>().AsQueryable();
            }

            var idValues = packageIds.Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var versionValues = versions.Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var targetFrameworkValues = String.IsNullOrEmpty(targetFrameworks) ? null : targetFrameworks.Split('|').Select(VersionUtility.ParseFrameworkName).ToList();

            // Exit early if the request looks invalid
            if ((idValues.Length == 0) || (idValues.Length != versionValues.Length))
                return Enumerable.Empty<Package>().AsQueryable();

            var packagesToUpdate = new List<IPackageMetadata>();
            for (int i = 0; i < idValues.Length; i++)
                packagesToUpdate.Add(new PackageBuilder { Id = idValues[i], Version = new SemanticVersion(versionValues[i]) });

            var repository = Repository;
            var versions2 = Backend.GetPackages(ref repository, "NuGet");
            var packages = versions2.Select(v => new Package(v, versions2)).AsQueryable();

            packages = packages.Where(p => p.IsLatestVersion && packagesToUpdate.Any(u => u.Id == p.Id));
            return packages;
        }
    }

    

    

    public sealed class UserTrackerModule : IHttpModule
    {
        void IHttpModule.Init(HttpApplication application)
        {
            application.PreRequestHandlerExecute += ApplicationOnPreRequestHandlerExecute;
        }

        void IHttpModule.Dispose()
        {
        }

        private void ApplicationOnPreRequestHandlerExecute(object sender, EventArgs eventArgs)
        {
            var application = (HttpApplication) sender;
            var wrapper = new HttpContextWrapper(application.Context);

            if (wrapper.Request.RequestContext.RouteData.RouteHandler is DynamicServiceRoute)
            {
                string company = (string)wrapper.Request.RequestContext.RouteData.Values["company"];
                string login = (string)wrapper.Request.RequestContext.RouteData.Values["login"];
                string key = (string)wrapper.Request.RequestContext.RouteData.Values["key"];
                string repository = (string)wrapper.Request.RequestContext.RouteData.Values["repository"];
                application.Context.Items.Add("Repository", new Repository {Company = company, Name = repository});
                var factory = ServiceLocator.Resolve<IGatewayBackendFactory<IPackageBackend>>();
                var manager = ServiceLocator.Resolve<INuGetGatewayManager>();
                var configurationFactory = ServiceLocator.Resolve<IGatewayConfigurationFactory>();

                if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(key))
                {
                    var caller = new Caller { Company = company, Name = login, KeyType = "VisualStudio", KeyValue = key };

                    if (factory.Validate(caller) != null)
                    {
                        application.Context.Items.Add("Caller", caller);
                        return;
                    }                        

                    application.Response.StatusCode = 403;
                    application.CompleteRequest();
                    return;
                }


                var auth = wrapper.Request.Headers["Authorization"];
                if (auth != null)
                {
                    var token = Encoding.ASCII.GetString(Convert.FromBase64String(auth.Split(' ')[1])).Split(':');
                    var caller = new Caller { Company = company, Name = token[0], KeyType = "Password", KeyValue = token[1] };

                    if (factory.Validate(caller) != null)
                    {
                        application.Context.Items.Add("Caller", caller);
                        return;
                    }                        
                }

                if (!manager.AuthenticateDownload(company, repository))
                {
                    var configuration = configurationFactory.Create(company);

                    var caller =  new Caller { Company = company, Name = configuration.PublicLogin, KeyType = "API", KeyValue = configuration.PublicPassword };
                    application.Context.Items.Add("Caller", caller);
                    return;
                }

                application.Response.StatusCode = 401;
                application.Response.AddHeader("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", company));
                application.CompleteRequest();

            }
        }

        public static void Register()
        {
            DynamicModuleUtility.RegisterModule(typeof(UserTrackerModule));
        }
    }
}
