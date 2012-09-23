using System;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Routing;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Gateway.NuGet.Core
{
    [RewriteBaseUrlBehavior]
    public class ODataPackageService : DataService<PackageContext>, IDataServiceStreamProvider, IServiceProvider
    {
        public static void MapRoutes(RouteCollection routes, string prefix, string suffix)
        {
            var factory = new DataServiceHostFactory();
            string servicePrefix = prefix + "/{repository}/FeedService.mvc";
            RouteTable.Routes.Add(new DynamicServiceRoute(servicePrefix, null, new[] { typeof(ODataPackageService).Namespace }, factory, typeof(ODataPackageService)));
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

        private PackageFilter GetFilter()
        {
            string filter = HttpContext.Current.Request.QueryString["$filter"];
            string skip = HttpContext.Current.Request.QueryString["$skip"] ?? "0";
            string top = HttpContext.Current.Request.QueryString["$top"] ?? "1000000";
            return new PackageFilter
                       {
                           Where = NuGetTranslator.TranslateFilter(filter),
                           OrderBy = NuGetTranslator.TranslateFilter(HttpContext.Current.Request.QueryString["$orderby"]),
                           Skip = int.Parse(skip),
                           Take = int.Parse(top),
                           Count = HttpContext.Current.Request.Path.EndsWith("$count")
                       };
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
            return new PackageContext(this);
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
            var filter = GetFilter();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim('\'');
                filter.Where = "(" + filter.Where + ") and (substringof(Project, '" + searchTerm + "') or substringof(Name, '" + searchTerm + "'))";
                return GetPackages(filter, v => v.Project.Contains(searchTerm) || v.Name.Contains(searchTerm));
            }

            return GetPackages(filter, v => true);
        }

        [WebGet]
        public IQueryable<Package> FindPackagesById(string id)
        {
            var filter = GetFilter();
            if (!string.IsNullOrEmpty(filter.Where))
                filter.Where = "(" + filter.Where + ") and Project eq '" + id + "'";
            else
                filter.Where = "Project eq '" + id + "'";
            return GetPackages(filter, v => v.Project == id);
        }

        [WebGet]
        public IQueryable<Package> GetUpdates(string packageIds, string versions, bool includePrerelease, bool includeAllVersions, string targetFrameworks)
        {
            if (String.IsNullOrEmpty(packageIds) || String.IsNullOrEmpty(versions))
            {
                return Enumerable.Empty<Package>().AsQueryable();
            }

            var idValues = packageIds.Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            string filterNames = string.Join(" or ", idValues.Select(i => "Project eq '" + i + "'"));
            var filter = GetFilter();
            
            if(!string.IsNullOrEmpty(filter.Where))
                filter.Where = "(" + filter.Where + ") and (" + filterNames + ") and (Metadata['IsLatestVersion'] eq 'True')";
            else
                filter.Where = "(" + filterNames + ") and (Metadata['IsLatestVersion'] eq 'True')";

            return GetPackages(filter, v => idValues.Contains(v.Project));
        }

        private IQueryable<Package> GetPackages(PackageFilter filter, Func<SymbolSource.Server.Management.Client.Version, bool> filter2)
        {
            var repository = Repository;
            var versions = Backend.GetPackages(ref repository, ref filter, "NuGet")
                .AsEnumerable();
            
            
            if (filter.Performed)
            {
                //If filter is performed on server side then skip skipping ;)
                (OperationContext.Current.IncomingMessageProperties["UriTemplateMatchResults"] as UriTemplateMatch).QueryParameters["$skip"] = "0";

                versions = versions.Where(filter2);
            }

            return versions
                .Select(NuGetTranslator.ConvertToPackage)
                .AsQueryable();
        }
    }
}