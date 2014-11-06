using System;
using System.Web.Mvc;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client.WebService;

namespace SymbolSource.Gateway.WinDbg.Core
{
    public class SourceController : Controller
    {
        private readonly IGatewayBackendFactory<IWinDbgBackend> factory;
        private readonly IGatewayConfigurationFactory configurationFactory;

        public SourceController(IGatewayBackendFactory<IWinDbgBackend> factory, IGatewayConfigurationFactory configurationFactory)
        {
            this.factory = factory;
            this.configurationFactory = configurationFactory;
        }

        public ActionResult Index(string company, string login, string password, string computerName, string computerUser, string imageName, string pdbHash, string sourcePath)
        {
            if ("Public".Equals(company, StringComparison.OrdinalIgnoreCase))
                company = "Public";

            if (string.IsNullOrEmpty(login) && string.IsNullOrEmpty(password))
            {
                var configuration = configurationFactory.Create(company);
                login = configuration.PublicLogin;
                password = configuration.PublicPassword;
            }

            //TODO: mamy na to test?
            //Ktoœ ma ma³¹ literk¹ numer kompilacji
            pdbHash = pdbHash.ToUpper();

            using (var backend = factory.Create(company, login, "VisualStudio", password))
            {
                var imageFile = backend.GetImageFile(imageName, pdbHash);

                if (imageFile == null)
                {
                    backend.LogImageFileNotFound(imageName, pdbHash);
                    Response.StatusCode = 404;
                    return Content(string.Format("{0} ({1}) not found", imageName, pdbHash));
                }

                var sourceFile = new SourceFile
                                     {
                                         Company = company,
                                         Repository = imageFile.Repository,
                                         Project = imageFile.Project,
                                         Version = imageFile.Version,
                                         Mode = imageFile.Mode,
                                         Platform = imageFile.Platform,
                                         ImageName = imageName,
                                         Path = sourcePath
                                     };
                var link = backend.GetSourceFileLink(ref sourceFile);

                if (link == null)
                {
                    Response.StatusCode = 404;
                    return Content("Source file not found");
                }

                backend.LogSourceFileFound(sourceFile, computerName, computerUser);
                return Redirect(link);
            }
        }
    }
}
