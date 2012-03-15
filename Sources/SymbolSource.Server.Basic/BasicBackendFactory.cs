using System;
using SymbolSource.Gateway.Core;
using SymbolSource.Processing.Basic.Projects;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Server.Basic
{
    public class BasicBackendFactory : IGatewayBackendFactory<BasicBackend>
    {
        private readonly IBasicBackendConfiguration configuration;
        private readonly IAddInfoBuilder addInfoBuilder;

        public BasicBackendFactory(IBasicBackendConfiguration configuration, IAddInfoBuilder addInfoBuilder)
        {
            this.configuration = configuration;
            this.addInfoBuilder = addInfoBuilder;
        }

        public BasicBackend Create(Caller caller)
        {
            return new BasicBackend(configuration, addInfoBuilder);
        }

        public User Validate(Caller caller)
        {
            return new User
                       {
                           Name = caller.Name,
                           Company = caller.Company,
                       };
        }

        public string DigestGenerateRequest(string realm)
        {
            throw new NotImplementedException();
        }

        public Caller DigestValidateResponse(string company, string method, string response)
        {
            throw new NotImplementedException();
        }

        public Caller GetUserByKey(string company, string type, string value)
        {
            return new Caller
                       {
                           Company = "Basic",
                           Name = "Basic",
                           KeyType = "Basic",
                           KeyValue = "Basic",
                       };
        }
    }
}