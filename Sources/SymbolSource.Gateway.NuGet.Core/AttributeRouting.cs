using System.Web.Routing;
using AttributeRouting.Web.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SymbolSource.Gateway.NuGet.Core.AttributeRouting), "Start")]

namespace SymbolSource.Gateway.NuGet.Core
{
    public static class AttributeRouting
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapAttributeRoutes();
            ODataPackageService.MapRoutes(routes, "{company}/{login}/{key}", "PreAuthenticated");
            ODataPackageService.MapRoutes(routes, "{company}", "");
        }

        public static void Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
