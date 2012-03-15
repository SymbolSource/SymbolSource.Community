namespace SymbolSource.Gateway.Core
{
    public class AppSettingsConfigurationFactory : IGatewayConfigurationFactory
    {
        public IGatewayConfiguration Create(string company)
        {
            return new AppSettingsConfiguration(company);
        }
    }
}