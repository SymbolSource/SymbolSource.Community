using System.ServiceModel;

namespace SymbolSource.Server.Management.Client
{
   //[WebServiceBinding(Name = "WebServiceSoap", Namespace = "http://api.symbolsource.org/schemas/webServices")]
    public class ConfigurableWebService : WebServiceClient
    {
        

        public ConfigurableWebService(IWebServiceManagementConfiguration configuration)
            : base("WS2007FederationHttpBinding_IWebService", new EndpointAddress(configuration.ManagementProxyPath))
        {            
            //Timeout = 99999999;
        }
    }
}
