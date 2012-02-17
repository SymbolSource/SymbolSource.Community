using System;

namespace SymbolSource.Gateway.Core
{
    public class ServerException : GatewayException
    {
        public ServerException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}