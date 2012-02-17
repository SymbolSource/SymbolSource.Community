using System;
using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.NuGet.Core
{
    public class Upload15Controller : Controller
    {
        private readonly IGatewayBackendFactory<IPackageBackend> factory;
        private readonly IGatewayManager manager;

        public Upload15Controller(IGatewayBackendFactory<IPackageBackend> factory, INuGetGatewayManager manager)
        {
            this.factory = factory;
            this.manager = manager;
        }

        private ActionResult DoAction(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (ClientException exception)
            {
                Response.StatusCode = 500;
                return Content(exception.ResponseContent);
            }
            catch (ServerException exception)
            {
                Response.StatusCode = 500;
                return Content(exception.ResponseContent);
            }
            catch (Exception exception)
            {
                Response.StatusCode = 500;
                return Content("Unexpected error: " + exception.Message + "\n\n" + exception);
            }
        }

        private Caller GetCaller(string company, string key)
        {
            var configuration = new ConfigurationWrapper(company);

            try
            {
                var caller = factory.GetUserByKey(company, "NuGet", key);

                if (caller != null)
                    return caller;

                if (string.IsNullOrEmpty(configuration.GatewayLogin) || string.IsNullOrEmpty(configuration.GatewayPassword))
                    throw new Exception("Missing gateway configuration");

                using (var backend = factory.Create(company, configuration.GatewayLogin, "API", configuration.GatewayPassword))
                    return backend.CreateUserByKey(company, "NuGet", key);
            }
            catch (Exception exception)
            {
                throw new ClientException("User authentication failure", exception);
            }
        }

        [POST("{company}/{repository}/PackageFiles/{key}/nupkg")]
        public ActionResult Push(string company, string repository, string key)
        {
            return DoAction(() => manager.Upload(GetCaller(company, key), Request.InputStream, company, repository));
        }

        [POST("{company}/{repository}/PublishedPackages/Publish")]
        public ActionResult Publish(string company, string repository, string id, string key, string version)
        {
            return DoAction(() => manager.Restore(GetCaller(company, key), company, repository, id, version));
        }

        [DELETE("{company}/{repository}/Packages/{key}/{project}/{version}")]
        public ActionResult Delete(string company, string repository, string key, string project, string version)
        {
            return DoAction(() => manager.Hide(GetCaller(company, key), company, repository, project, version));
        }
    }
}
