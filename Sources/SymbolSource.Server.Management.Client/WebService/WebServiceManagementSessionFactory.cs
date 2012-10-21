using System;

namespace SymbolSource.Server.Management.Client
{
    public class WebServiceManagementSessionFactory : IManagementSessionFactory
    {
        protected readonly IWebServiceManagementConfiguration configuration;
        protected readonly IWebService service;

        public WebServiceManagementSessionFactory(IWebServiceManagementConfiguration configuration)
        {
            this.configuration = configuration;
            service = new ConfigurableWebService(configuration);
        }

        public virtual IManagementSession Create(Caller caller)
        {
            return new WebServiceManagementSession(configuration, caller);
        }

        public User Validate(Caller caller)
        {
            return service.UserValidate(caller);
        }

        public string DigestGenerateRequest(string realm)
        {
            return service.DigestGenerateRequest(realm);
        }

        public Caller DigestValidateResponse(string company, string method, string response)
        {
            return service.DigestValidateResponse(company, method, response);
        }


        public Caller GetUserByKey(string company, string type, string value)
        {
            //tu musze dac geta w webservisie:
            return service.GetUserByKey(company, type, value);
        }
    }
}