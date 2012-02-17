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
            DownloadController.MapRoutes(routes, "{company}/{login}/{key}", "PreAuthenticated");
            DownloadController.MapRoutes(routes, "{company}", "");
        }

        public static void Start()
        {
            RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders[typeof(ODataUrlQueryOptions)] = new ODataUrlQueryOptionsModelBinder();
        }
    }
}
