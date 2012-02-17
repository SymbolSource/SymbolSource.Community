using System;
using SymbolSource.Gateway.Core;
using SymbolSource.Gateway.WinDbg.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend : IWinDbgBackend, IPackageBackend
    {
        private readonly IBasicBackendConfiguration configuration;

        public BasicBackend(IBasicBackendConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Caller GetOrCreateUserByKey(string company, string type, string value)
        {
            throw new NotImplementedException();
        }
    }
}