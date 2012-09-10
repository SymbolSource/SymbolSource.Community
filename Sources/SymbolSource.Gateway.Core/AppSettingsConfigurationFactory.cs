namespace SymbolSource.Gateway.Core
{
    public class AppSettingsConfigurationFactory : IGatewayConfigurationFactory
    {
        public IGatewayConfiguration Create(string company)
        {
            return new AppSettingsConfiguration(company);
        }

        public IGatewayRepositoryConfiguration Create(string company, string repository)
        {
            return new AppSettingsRepositoryConfiguration(company, repository);
        }
    }
}