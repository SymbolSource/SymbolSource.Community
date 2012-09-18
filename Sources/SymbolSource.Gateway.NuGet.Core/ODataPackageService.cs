using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Routing;
using NuGet;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.NuGet.Core
{
    [RewriteBaseUrlBehavior]
    public class ODataPackageService : DataService<PackageContext>, IDataServiceStreamProvider, IServiceProvider
    {
        public static void MapRoutes(RouteCollection routes, string prefix, string suffix)
        {
            var factory = new DataServiceHostFactory();
            string servicePrefix = prefix + "/{repository}/FeedService.mvc";
            RouteTable.Routes.Add(new DynamicServiceRoute(servicePrefix, null, factory, typeof(ODataPackageService)));
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
            //IEnumerable<string> targetFrameworks = String.IsNullOrEmpty(targetFramework) ? Enumerable.Empty<string>() : targetFramework.Split('|');

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
}