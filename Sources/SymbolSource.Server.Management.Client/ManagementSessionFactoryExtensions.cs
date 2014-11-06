using System;
using SymbolSource.Server.Management.Client.WebService;

namespace SymbolSource.Server.Management.Client.WebService
{
    public static class ManagementSessionFactoryExtensions
    {
        [Obsolete]
        public static IManagementSession Create(this IManagementSessionFactory factory, string company, string name, string keyType, string keyValue)
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