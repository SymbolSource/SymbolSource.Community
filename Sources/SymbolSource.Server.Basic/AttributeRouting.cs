using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivator.PostApplicationStartMethod(typeof(SymbolSource.Server.Basic.AttributeRouting), "Start")]

namespace SymbolSource.Server.Basic
{
    public static class AttributeRouting
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            foreach (var route in routes.OfType<Route>())
            {
                var namespaces = (string[])route.DataTokens["namespaces"];

                if (namespaces != null)
                {
                    route.Url = Regex.Match(namespaces[0], "^SymbolSource.Gateway.([^.]+).Core$").Groups[1] + "/" + route.Url;
                    ReplaceToken(route, "login", "Basic");
                    ReplaceToken(route, "company", "Basic");
                    ReplaceToken(route, "repository", "Basic");
                }
            }

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

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
