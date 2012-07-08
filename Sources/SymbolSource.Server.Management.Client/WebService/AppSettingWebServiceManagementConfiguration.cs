using System;
using System.Configuration;

namespace SymbolSource.Server.Management.Client
{
    public class AppSettingWebServiceManagementConfiguration : IWebServiceManagementConfiguration
    {
        public string ManagementProxyPath
        {
            get { return IsRedirected ? ConfigurationManager.AppSettings["ManagementRedirect"] : ConfigurationManager.AppSettings["ManagementProxyPath"]; }
        }

        public bool IsRedirected
        {
            get { return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ManagementRedirect"]); }
        }
    }
}