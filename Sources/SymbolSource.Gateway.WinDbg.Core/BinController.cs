using System;
using System.IO;
using System.Web.Mvc;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client.WebService;

namespace SymbolSource.Gateway.WinDbg.Core
{
    public class BinController : Controller
    {
        private readonly IGatewayBackendFactory<IWinDbgBackend> factory;
        private readonly IGatewayConfigurationFactory configurationFactory;

        public BinController(IGatewayBackendFactory<IWinDbgBackend> factory, IGatewayConfigurationFactory configurationFactory)
        {
            this.factory = factory;
            this.configurationFactory = configurationFactory;
        }

        public ActionResult Index(string company, string login, string password, string name, string hash, string name1)
        {
            if ("Public".Equals(company, StringComparison.OrdinalIgnoreCase))
                company = "Public";

            if (string.IsNullOrEmpty(login) && string.IsNullOrEmpty(password))
            {
                var configuration = configurationFactory.Create(company);
                login = configuration.PublicLogin;
                password = configuration.PublicPassword;
            }

            if (!name1.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) && !name1.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
            {
                Response.StatusCode = 404;
                return Content("Supported only not compresioned files (.dll/.exe)");
            }

            //Ktoś ma małą literką numer kompilacji
            hash = hash.ToUpper();

            string imageName = Path.GetFileNameWithoutExtension(name);

            using (var backend = factory.Create(company, login, "VisualStudio", password))
            {
                var imageFile = backend.GetImageFile(imageName, hash);

                if (imageFile != null)
                    return HandleFound(backend, imageFile);
            }

            if (company != "Public")
            {
                var configuration = new AppSettingsConfiguration("Public");

                using (var backend = factory.Create("Public", configuration.PublicLogin, "VisualStudio", configuration.PublicPassword))
                {
                    var imageFile = backend.GetImageFile(imageName, hash);

                    if (imageFile != null)
                        return HandleFound(backend, imageFile);
                }
            }

            using (var backend = factory.Create(company, login, "VisualStudio", password))
                return HandleNotFound(backend, name, hash, imageName);
        }

        private ActionResult HandleNotFound(IWinDbgBackend backend, string name, string hash, string imageName)
        {
            //TODO: czy tutaj nie lepiej logować name niż imageName?
            backend.LogImageFileNotFound(imageName, hash);
            Response.StatusCode = 404;
            return Content(string.Format("{0} ({1}) not found", name, hash));
        }

        private ActionResult HandleFound(IWinDbgBackend backend, ImageFile imageFile)
        {
            backend.LogImageFileFound(imageFile);
            string url = backend.GetImageFileLink(ref imageFile);
            return Redirect(url);
        }
    }
}
