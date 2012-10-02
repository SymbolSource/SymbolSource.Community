using System.Web.Routing;
using AttributeRouting.Web.Mvc;

[assembly: WebActivator.PreApplicationStartMethod(typeof(SymbolSource.Gateway.OpenWrap.Core.AttributeRouting), "Start")]

namespace SymbolSource.Gateway.OpenWrap.Core
{
    public static class AttributeRouting
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapAttributeRoutes();
        }

        public static void Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
