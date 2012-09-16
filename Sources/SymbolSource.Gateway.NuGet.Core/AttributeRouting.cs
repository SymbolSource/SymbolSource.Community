using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting;
using SymbolSource.OData;

[assembly: WebActivator.PreApplicationStartMethod(typeof(SymbolSource.Gateway.NuGet.Core.AttributeRouting), "Start")]

namespace SymbolSource.Gateway.NuGet.Core
{
    public static class AttributeRouting
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapAttributeRoutes();
            Packages.MapRoutes(routes, "{company}/{login}/{key}", "PreAuthenticated");
            Packages.MapRoutes(routes, "{company}", "");
            DownloadController.MapRoutes(routes, "{company}/{login}/{key}", "PreAuthenticated2");
            DownloadController.MapRoutes(routes, "{company}", "2");
        }

        public static void Start()
        {
            RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders[typeof(ODataUrlQueryOptions)] = new ODataUrlQueryOptionsModelBinder();
        }
    }
}
