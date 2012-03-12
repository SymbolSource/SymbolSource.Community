using System;
using SymbolSource.Gateway.Core;
using SymbolSource.Gateway.WinDbg.Core;
using SymbolSource.Processing.Basic.Projects;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend : IWinDbgBackend, IPackageBackend
    {
        private readonly IBasicBackendConfiguration configuration;
        private readonly IAddInfoBuilder addInfoBuilder;

        public BasicBackend(IBasicBackendConfiguration configuration, IAddInfoBuilder addInfoBuilder)
        {
            this.configuration = configuration;
            this.addInfoBuilder = addInfoBuilder;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Caller GetOrCreateUserByKey(string company, string type, string value)
        {
            throw new NotImplementedException();
        }
    }
}