using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SymbolSource.Gateway.Core;

namespace SymbolSource.Gateway.WinDbg.Core
{
    public class DefaultController : Controller
    {
        private readonly IGatewayConfigurationFactory configurationFactory;

        public DefaultController(IGatewayConfigurationFactory configurationFactory)
        {
            this.configurationFactory = configurationFactory;
        }

        public ActionResult Index(string company, string login, string password, string name, string hash, string name1)
        {
            if ("Public".Equals(company, StringComparison.OrdinalIgnoreCase))
                company = "Public";

            if (string.IsNullOrEmpty(login) && string.IsNullOrEmpty(password))
            {
                var configuration = configurationFactory.Create(company);
                login = configuration.PublicLogin;
                password = configuration.PublicPassword;
            }

            string extension = Path.GetExtension(name);

            var routeValue = new RouteValueDictionary(new {company, login, password, name, hash, name1});

            if(string.Equals(".pdb", extension, StringComparison.InvariantCultureIgnoreCase))
                return new TransferToRouteResult("Pdb", routeValue);
            else if(string.Equals(".exe", extension, StringComparison.InvariantCultureIgnoreCase) || string.Equals(".dll", extension, StringComparison.InvariantCultureIgnoreCase))
                return new TransferToRouteResult("Bin", routeValue);

            Response.StatusCode = 404;
            return Content(string.Format("{0} not supported", extension));
        }

        public ActionResult Index404()
        {
            Response.StatusCode = 404;
            return null;
        }
    }

    public class TransferResult : RedirectResult
    {
        public TransferResult(string url)
            : base(url)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var httpContext = HttpContext.Current;

            // MVC 3 running on IIS 7+
            if (HttpRuntime.UsingIntegratedPipeline)
            {
                httpContext.Server.TransferRequest(Url, true);
            }
            else
            {
                // Pre MVC 3
                httpContext.RewritePath(Url, false);

                IHttpHandler httpHandler = new MvcHttpHandler();
                httpHandler.ProcessRequest(httpContext);
            }
        }
    }

    public class TransferToRouteResult : ActionResult
    {
        public string RouteName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }

        public TransferToRouteResult(RouteValueDictionary routeValues)
            : this(null, routeValues)
        {
        }

        public TransferToRouteResult(string routeName, RouteValueDictionary routeValues)
        {
            RouteName = routeName ?? string.Empty;
            RouteValues = routeValues ?? new RouteValueDictionary();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var urlHelper = new UrlHelper(context.RequestContext);
            var url = urlHelper.RouteUrl(this.RouteName, this.RouteValues);

            var actualResult = new TransferResult(url);
            actualResult.ExecuteResult(context);
        }
    }
}
