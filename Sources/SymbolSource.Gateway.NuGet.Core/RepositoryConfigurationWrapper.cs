using System.Configuration;

namespace SymbolSource.Gateway.NuGet.Core
{
    public class RepositoryConfigurationWrapper
    {
        private readonly string company;
        private readonly string repository;

        public RepositoryConfigurationWrapper(string company, string repository)
        {
            this.company = company;
            this.repository = repository;
        }

        public string Service
        {
            get { return ConfigurationManager.AppSettings[company + repository + "Service"]; }
        }
    }
}