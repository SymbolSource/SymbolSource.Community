using System.Web.Mvc;

namespace SymbolSource.Gateway.NuGet.Core
{
    public class DataServiceAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;

            if (request.Headers["MaxDataServiceVersion"] == "malformed" || request.Headers["DataServiceVersion"] == "9999.99" || request.Headers["DataServiceVersion"] == "m.n")
            {
                response.StatusCode = 400;
                filterContext.Result = new ContentResult();
            }
            else
            {
                response.AddHeader("DataServiceVersion", "1.0;");
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}