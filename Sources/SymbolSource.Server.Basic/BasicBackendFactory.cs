using System;
using SymbolSource.Gateway.Core;
using SymbolSource.Gateway.WinDbg.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Server.Basic
{
    public class BasicBackendFactory : IGatewayBackendFactory<BasicBackend>
    {
        private readonly IBasicBackendConfiguration configuration;

        public BasicBackendFactory(IBasicBackendConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public BasicBackend Create(Caller caller)
        {
            return new BasicBackend(configuration);
        }

        public User Validate(Caller caller)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}