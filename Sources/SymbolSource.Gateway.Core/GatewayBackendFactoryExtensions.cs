
using System;
using SymbolSource.Server.Management.Client.WebService;

namespace SymbolSource.Gateway.Core
{
    public static class GatewayBackendFactoryExtensions
    {
        [Obsolete]
        public static T Create<T>(this IGatewayBackendFactory<T> factory, string company, string name, string keyType, string keyValue) where T : IGatewayBackend
        {
            var caller = new Caller
            {
                Company = company,
                Name = name,
                KeyType = keyType,
                KeyValue = keyValue,
            };

            return factory.Create(caller);
        }
    }
}
