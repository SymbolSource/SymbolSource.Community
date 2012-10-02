using System;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.NuGet.Core
{
    public class Upload16Controller : Controller
    {
        private readonly IGatewayBackendFactory<IPackageBackend> backendFactory;
        private readonly IGatewayConfigurationFactory configurationFactory;
        private readonly IGatewayManager manager;

        public Upload16Controller(IGatewayBackendFactory<IPackageBackend> backendFactory, IGatewayConfigurationFactory configurationFactory, INuGetGatewayManager manager)
        {
            this.backendFactory = backendFactory;
            this.configurationFactory = configurationFactory;
            this.manager = manager;
        }

        private ActionResult DoAction(Action action, int successCode, string successMessage)
        {
            try
            {
                action();
                Response.StatusCode = successCode;
                Response.StatusDescription = successMessage;
                return Content("");
            }
            catch (ClientException exception)
            {
                Response.StatusCode = 418;
                Response.StatusDescription = exception.ResponseStatusDescription;
                return Content(exception.ResponseContent);
            }
            catch (ServerException exception)
            {
                Response.StatusCode = 506;
                Response.StatusDescription = exception.ResponseStatusDescription;
                return Content(exception.ResponseContent);
            }
            catch (Exception exception)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Unexpected error: " + exception.Message;
                return Content("Unexpected error: " + exception.Message + "\n\n" + exception);
            }
        }

        private Caller GetCaller(string company, string key)
        {
            var configuration = configurationFactory.Create(company);

            try
            {
                var caller = backendFactory.GetUserByKey(company, "NuGet", key);

                if (caller != null)
                    return caller;

                if (string.IsNullOrEmpty(configuration.GatewayLogin) || string.IsNullOrEmpty(configuration.GatewayPassword))
                    throw new Exception("Wrong key or missing gateway configuration");

                using (var backend = backendFactory.Create(company, configuration.GatewayLogin, "API", configuration.GatewayPassword))
                    return backend.CreateUserByKey(company, "NuGet", key);
            }
            catch (Exception exception)
            {
                throw new ClientException("User authentication failure", exception);
            }
        }

        [PUT("{company}/{repository}")]
        public ActionResult Push(string company, string repository)
        {
            var key = Request.Headers["X-NuGet-ApiKey"];

            return DoAction(
                () => manager.Upload(GetCaller(company, key), Request.Files[0].InputStream, company, repository),
                201, "Submission successful.");
        }

        [DELETE("{company}/{repository}/{project}/{version}")]
        public ActionResult Delete(string company, string repository, string project, string version)
        {
            var key = Request.Headers["X-NuGet-ApiKey"];
            
            return DoAction(
                 () => manager.Hide(GetCaller(company, key), company, repository, project, version),
                 200, "Package is now hidden. It will not show up on any listing, but PDBs and sources will still be served for clients with old binaries. Use the SymbolSource website to delete pernamently.");
        }
    }
}
