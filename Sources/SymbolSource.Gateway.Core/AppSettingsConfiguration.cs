using System.Configuration;

namespace SymbolSource.Gateway.Core
{
    public class AppSettingsConfiguration : IGatewayConfiguration
    {
        private readonly string company;

        public AppSettingsConfiguration(string company)
        {
            this.company = company;
        }

        public string GatewayLogin
        {
            get { return ConfigurationManager.AppSettings[company + "GatewayLogin"]; }
        }

        public string GatewayPassword
        {
            get { return ConfigurationManager.AppSettings[company + "GatewayPassword"]; }
        }

        public string PublicLogin
        {
            get { return ConfigurationManager.AppSettings[company + "PublicLogin"]; }
        }

        public string PublicPassword
        {
            get { return ConfigurationManager.AppSettings[company + "PublicPassword"]; }
        }
    }
}