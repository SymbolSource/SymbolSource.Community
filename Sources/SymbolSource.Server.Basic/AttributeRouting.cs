using System.Text.RegularExpressions;
using System.Web.Routing;
using SymbolSource.Gateway.NuGet.Core;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(SymbolSource.Server.Basic.AttributeRouting), "Start")]

namespace SymbolSource.Server.Basic
{
    public static class AttributeRouting
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapAttributeRoutes();

            foreach (var routeBase in routes)
            {
                var route = routeBase as Route;
                if (routeBase is DynamicServiceRoute)
                    route = ((DynamicServiceRoute) routeBase).InnerRoute;

                if(route == null)
                    return;

                if(route.DataTokens == null)
                    continue;

                var namespaces = route.DataTokens["namespaces"] as string[];

                if (namespaces != null)
                {
                    route.Url = Regex.Match(namespaces[0], "^SymbolSource.Gateway.([^.]+).Core$").Groups[1] + "/" + route.Url;
                    ReplaceToken(route, "login", "Basic");
                    ReplaceToken(route, "company", "Basic");
                    ReplaceToken(route, "password", "Basic");
                    ReplaceToken(route, "repository", "Basic");
                }
            }
        }

        private static void ReplaceToken(Route route, string key, string value)
        {
            route.Defaults[key] = value;
            route.Url = route.Url.Replace("{" + key + "}", "").Replace("//", "/");
        }

        public static void Start()
        {
            RegisterRoutes(RouteTable.Routes);
            MicroKernel.Install();
        }
    }
}
