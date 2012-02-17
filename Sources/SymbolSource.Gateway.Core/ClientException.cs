using System;

namespace SymbolSource.Gateway.Core
{
    public class ClientException : GatewayException
    {
        public ClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}