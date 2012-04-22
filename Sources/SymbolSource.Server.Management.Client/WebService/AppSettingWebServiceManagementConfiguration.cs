using System;
using System.Configuration;

namespace SymbolSource.Server.Management.Client
{
    public class AppSettingWebServiceManagementConfiguration : IWebServiceManagementConfiguration
    {
        public string ManagementProxyPath
        {
            get { return ConfigurationManager.AppSettings["ManagementProxyPath"]; }
            set { throw new NotImplementedException(); }
        }
    }
}