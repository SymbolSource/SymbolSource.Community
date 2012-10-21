using System.Security.Policy;
using System.ServiceModel;
using System.Threading;
using System.Web.Services;

namespace SymbolSource.Server.Management.Client
{
   //[WebServiceBinding(Name = "WebServiceSoap", Namespace = "http://api.symbolsource.org/schemas/webServices")]
    public class ConfigurableWebService : WebServiceClient
    {
        

        public ConfigurableWebService(IWebServiceManagementConfiguration configuration)
            :base (new BasicHttpBinding(), new EndpointAddress(configuration.ManagementProxyPath))
        {
            //Url = configuration.ManagementProxyPath;
            //Timeout = 99999999;
        }
    }
}
