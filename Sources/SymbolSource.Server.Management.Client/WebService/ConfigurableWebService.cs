using System.Web.Services;

namespace SymbolSource.Server.Management.Client.WebService
{
    [WebServiceBinding(Name = "WebServiceSoap", Namespace = "http://api.symbolsource.org/schemas/webServices")]
    public class ConfigurableWebService : WebService
    {
        public ConfigurableWebService(IWebServiceManagementConfiguration configuration)
        {
            Url = configuration.ManagementProxyPath;
            Timeout = 99999999;
        }
    }
}
