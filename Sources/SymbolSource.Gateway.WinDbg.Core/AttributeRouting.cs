using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting.Web.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SymbolSource.Gateway.WinDbg.Core.AttributeRouting), "Start")]

namespace SymbolSource.Gateway.WinDbg.Core
{
    public static class AttributeRouting
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapAttributeRoutes();

            RegisterWinDbg(routes, "Pdb");
            RegisterWinDbg(routes, "Bin");

            routes.MapRoute(
                "Source",
                "pdbsrc/{company}/{login}/{password}/{computerName}/{computerUser}/{imageName}/{pdbHash}/{*sourcePath}",
                new { controller = "Source", action = "Index" },
                new[] { typeof(AttributeRouting).Namespace }
                );

            routes.MapRoute(
                "Default-404",
                "{company}/{login}/{password}/index2.txt",
                new { controller = "Default", action = "Index404" },
                new[] { typeof(AttributeRouting).Namespace }
                );

            routes.MapRoute(
                "H-Public-404",
                "{company}/index2.txt",
                new { controller = "Default", action = "Index404" },
                new[] { typeof(AttributeRouting).Namespace }
                );

            routes.MapRoute(
                "Default-Default",
                "{company}/{login}/{password}/{name}/{hash}/{name1}",
                new { controller = "Default", action = "Index" },
                new[] { typeof(AttributeRouting).Namespace }
                );

            routes.MapRoute(
                "Default-Public",
                "{company}/{name}/{hash}/{name1}",
                new { controller = "Default", action = "Index" },
                new[] { typeof(AttributeRouting).Namespace }
                );
        }

        private static void RegisterWinDbg(RouteCollection routes, string type)
        {
            routes.MapRoute(
                type + "-404",
                type.ToLower() + "/{company}/{login}/{password}/index2.txt",
                new { controller = "Default", action = "Index404" },
                new[] { typeof(AttributeRouting).Namespace }
                );

            routes.MapRoute(
                type + "-Public-404",
                type.ToLower() + "/{company}/index2.txt",
                new { controller = "Default", action = "Index404" },
                new[] { typeof(AttributeRouting).Namespace }
                );

            routes.MapRoute(
                type,
                type.ToLower() + "/{company}/{login}/{password}/{name}/{hash}/{name1}",
                new { controller = type, action = "Index" },
                new[] { typeof(AttributeRouting).Namespace }
                );

            routes.MapRoute(
                type + "-Public",
                type.ToLower() + "/{company}/{name}/{hash}/{name1}",
                new { controller = type, action = "Index" },
                new[] { typeof(AttributeRouting).Namespace }
                );
        }

        public static void Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
