using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using SymbolSource.Gateway.Core;
using SymbolSource.OData;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.NuGet.Core
{
    public class DownloadController : Controller
    {
        public static void MapRoutes(RouteCollection routes, string prefix, string suffix)
        {
            //RegisterFeedServiceRoutes(routes, suffix, prefix + "/{repository}/FeedService.svc");
            RegisterFeedServiceRoutes(routes, suffix, prefix + "/{repository}/FeedService.mvc");
        }

        private static void RegisterFeedServiceRoutes(RouteCollection routes, string suffix, string servicePrefix)
        {
            routes.MapRoute("Package" + suffix, servicePrefix + "/Packages/{id}", new { controller = "Download", action = "List" }, new[] { typeof(DownloadController).Namespace });

            routes.MapRoute("CountFilter" + suffix, servicePrefix + "/Packages()/$count", new { controller = "Download", action = "List", count = true }, new[] { typeof(DownloadController).Namespace });
            routes.MapRoute("ListFilter" + suffix, servicePrefix + "/Packages()", new { controller = "Download", action = "List", count = false }, new[] { typeof(DownloadController).Namespace });

            routes.MapRoute("CountSearch" + suffix, servicePrefix + "/Search()/$count", new { controller = "Download", action = "Search", count = true }, new[] { typeof(DownloadController).Namespace });
            routes.MapRoute("ListSearch" + suffix, servicePrefix + "/Search()", new { controller = "Download", action = "Search", count = false }, new[] { typeof(DownloadController).Namespace });

            routes.MapRoute("Download" + suffix, servicePrefix + "/Packages/{project}/{version}", new { controller = "Download", action = "Download" }, new[] { typeof(DownloadController).Namespace });

            routes.MapRoute("List" + suffix, servicePrefix + "/Packages", new { controller = "Download", action = "List" }, new[] { typeof(DownloadController).Namespace });

            routes.MapRoute("Metadata" + suffix, servicePrefix + "/$metadata", new { controller = "Download", action = "Metadata" }, new[] { typeof(DownloadController).Namespace });
            routes.MapRoute("Feed" + suffix, servicePrefix + "", new { controller = "Download", action = "Index" }, new[] { typeof(DownloadController).Namespace });
        }

        private readonly XNamespace nsDataServices = "http://schemas.microsoft.com/ado/2007/08/dataservices";
        private readonly XNamespace nsMetadata = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private readonly XNamespace nsAtom = "http://www.w3.org/2005/Atom";
        private readonly XNamespace nsApp = "http://www.w3.org/2007/app";

        private readonly IGatewayBackendFactory<IPackageBackend> factory;
        private readonly IGatewayManager manager;

        public DownloadController(IGatewayBackendFactory<IPackageBackend> factory , INuGetGatewayManager manager)
        {
            this.factory = factory;
            this.manager = manager;
        }

        private static string GetPackageId(string id, string version)
        {
            return string.Format("Package(Id='{0}',Version='{1}')", id, version);
        }
        
        #region Links

        private string GetFeedLink(string company, string login, string key, string repository)
        {
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(key))
                return Url.RouteUrl("FeedPreAuthenticated", new { company, login, key, repository }, Request.Url.Scheme);

            return Url.RouteUrl("Feed", new { company, repository }, Request.Url.Scheme);
        }

        private string GetListLink(string company, string login, string key, string repository)
        {
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(key))
                return Url.RouteUrl("ListPreAuthenticated", new { company, login, key, repository }, Request.Url.Scheme);

            return Url.RouteUrl("List", new { company, repository }, Request.Url.Scheme);
        }

        private string GetDownloadLink(string company, string login, string key, string repository, string project, string version)
        {
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(key))
                return Url.RouteUrl("DownloadPreAuthenticated", new { company, login, key, repository, project, version }, Request.Url.Scheme);

            return Url.RouteUrl("Download", new { company, repository, project, version }, Request.Url.Scheme);
        }

        private string GetPackageLink(string company, string login, string key, string repository, string project, string version)
        {
            var id = GetPackageId(project, version);

            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(key))
                return Url.RouteUrl("PackagePreAuthenticated", new { company, login, key, repository, id }, Request.Url.Scheme);

            return Url.RouteUrl("Package", new { company, repository, id }, Request.Url.Scheme);
        }

        #endregion

        private Caller Authorize(string company, string login, string key, bool require)
        {
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(key))
            {
                var caller = new Caller { Company = company, Name = login, KeyType = "VisualStudio", KeyValue = key };

                if (factory.Validate(caller) != null)
                    return caller;

                Response.StatusCode = 403;
                return null;
            }

            var auth = Request.Headers["Authorization"];

            if (auth != null)
            {
                var token = Encoding.ASCII.GetString(Convert.FromBase64String(auth.Split(' ')[1])).Split(':');
                var caller = new Caller { Company = company, Name = token[0], KeyType = "Password", KeyValue = token[1] };

                if (factory.Validate(caller) != null)
                    return caller;
            }

            if (!require)
            {
                var configuration = new ConfigurationWrapper(company);

                return new Caller
                {
                    Company = company,
                    Name = configuration.PublicLogin,
                    KeyType = "API",
                    KeyValue = configuration.PublicPassword
                };
            }

            Response.StatusCode = 401;
            Response.AddHeader("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", company));
            return null;
        }

        [DataService]
        public ActionResult Index(string company, string login, string key, string repository)
        {
            var caller = Authorize(company, login, key, manager.Authorize(company, repository));

            if (caller == null)
                return null;

            return Content(
                new XDocument(
                    new XElement(
                        nsApp + "service",
                        new XAttribute(XNamespace.Xml + "base", GetFeedLink(company, login, key, repository)),
                        new XElement(
                            nsApp + "workspace",
                            new XElement(nsAtom + "title", "Default"),
                            new XElement(nsApp + "collection", new XAttribute("href", "Packages"), new XElement(nsAtom + "title", "Packages")))
                        )).ToString(),
                "application/xml");
        }

        [DataService]
        public ActionResult Metadata(string company, string login, string key, string repository)
        {
            var caller = Authorize(company, login, key, manager.Authorize(company, repository));

            if (caller == null)
                return null;

            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + "." + "Metadata.xml"))
            using (var reader = new StreamReader(stream))
                return Content(reader.ReadToEnd(), "application/xml");
        }

        [DataService]
        public ActionResult List(string company, string login, string key, string repository, ODataUrlQueryOptions options, bool count)
        {
            return Search(company, login, key, repository, null, options, count);
        }

        [DataService]
        public ActionResult Search(string company, string login, string key, string repository, string searchTerm, ODataUrlQueryOptions options, bool count)
        {
            var caller = Authorize(company, login, key, manager.Authorize(company, repository));

            if (caller == null)
                return null;

            var packages = manager.Index(caller, company, repository);
            var packagesAdapters = packages
                .AsQueryable()
                .Select(p => new PackageAdapter(p, packages));

            packagesAdapters = packagesAdapters.Process(options);

            if(!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim('\'');
                packagesAdapters = packagesAdapters.Where(p => p.Title.Contains(searchTerm) || p.Version.Contains(searchTerm));
            }

            var result = packagesAdapters
                .ToArray();

            if (count)
                return Content(result.Length.ToString());
            else
                return Odata(company, login, key, repository, result);
        }

        [DataService]
        public ActionResult Count(string company, string login, string key, string repository, string filter)
        {
            var caller = Authorize(company, login, key, manager.Authorize(company, repository));

            if (caller == null)
                return null;

            var packages = manager.Index(caller, company, repository);
            var packagesAdapters = packages
                .AsQueryable()
                .Select(p => new PackageAdapter(p, packages))
                .Where(filter)
                .ToArray();

            return Content(packagesAdapters.Count().ToString());
        }

        private ContentResult Odata(string company, string login, string key, string repository, IEnumerable<PackageAdapter> packages)
        {
            return Content(
                new XDocument(
                    new XElement(
                        nsAtom + "feed",
                        new XAttribute(XNamespace.Xml + "base", GetFeedLink(company, login, key, repository)),
                        new XElement(nsAtom + "id", GetListLink(company, login, key, repository)),
                        new XElement(
                            nsAtom + "link",
                            new XAttribute("rel", "self"),
                            new XAttribute("title", "Packages"),
                            new XAttribute("href", "Packages")),
                        new XElement(
                            nsMetadata + "count",
                            1),
                        packages.Select(package => GetPackageElement(company, login, key, repository, package)))).ToString(),
                "application/xml");
        }

        private XElement GetPackageElement(string company, string login, string key, string repository, PackageAdapter package)
        {
            return new XElement(
                nsAtom + "entry",
                new XElement(nsAtom + "id", GetPackageLink(company, login, key, repository, package.Title, package.Version)),
                new XElement(nsAtom + "title", new XAttribute("type", "text"), package.Title),
                new XElement(
                    nsAtom + "link",
                    new XAttribute("rel", "edit-media"),
                    new XAttribute("title", "PublishedPackage"),
                    new XAttribute("href", GetPackageId(package.Title, package.Version))),
                new XElement(
                    nsAtom + "link",
                    new XAttribute("rel", "edit"),
                    new XAttribute("title", "PublishedPackage"),
                    new XAttribute("href", GetPackageId(package.Title, package.Version))),
                new XElement(
                    nsAtom + "category",
                    new XAttribute("term", "Gallery.Infrastructure.FeedModels.PublishedPackage"),
                    new XAttribute("scheme", "http://schemas.microsoft.com/ado/2007/08/dataservices/scheme")),
                new XElement(
                    nsAtom + "content",
                    new XAttribute("type", "application/zip"),
                    new XAttribute("src", GetDownloadLink(company, login, key, repository, package.Title, package.Version))),
                new XElement(
                    nsMetadata + "properties",
                    new XElement(nsDataServices + "Id", package.Id),
                    new XElement(nsDataServices + "Version", package.Version),
                    new XElement(nsDataServices + "Title", package.Title),
                    new XElement(nsDataServices + "Authors", package.Authors),                    
                    new XElement(nsDataServices + "PackageType", package.PackageType),
                    new XElement(nsDataServices + "IsLatestVersion", package.IsLatestVersion),
                    new XElement(nsDataServices + "Published", package.Published),
                    new XElement(nsDataServices + "PackageHash", package.PackageHash),
                    new XElement(nsDataServices + "PackageHashAlgorithm", package.PackageHashAlgorithm),
                    new XElement(nsDataServices + "DownloadCount", package.DownloadCount),
                    new XElement(nsDataServices + "VersionDownloadCount", package.DownloadCount),
                    new XElement(nsDataServices + "PackageSize", package.PackageSize)
                    )
                );
        }

        public ActionResult Download(string company, string login, string key, string repository, string project, string version)
        {
            var caller = Authorize(company, login, key, manager.Authorize(company, repository));

            if (caller == null)
                return null;

            var contentType = Request.ContentType;

            if (contentType == "")
                contentType = null;

            return Redirect(manager.Download(caller, company, repository, project, version, contentType));
        }       
    }
}
