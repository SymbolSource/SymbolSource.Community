using System.Configuration;

namespace SymbolSource.Gateway.Core
{
    public interface IGatewayConfiguration
    {
        string GatewayLogin { get; }
        string GatewayPassword { get; }
        string PublicLogin { get; }
        string PublicPassword { get; }
    }

    public class ConfigurationWrapper : IGatewayConfiguration
    {
        private readonly string company;

        public ConfigurationWrapper(string company)
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