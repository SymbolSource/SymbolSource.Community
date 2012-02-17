using System;
using System.Web.Mvc;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.WinDbg.Core
{
    public class SourceController : Controller
    {
        private readonly IGatewayBackendFactory<IWinDbgBackend> factory;

        public SourceController(IGatewayBackendFactory<IWinDbgBackend> factory)
        {
            this.factory = factory;
        }

        public ActionResult Index(string company, string login, string password, string computerName, string computerUser, string imageName, string pdbHash, string sourcePath)
        {
            if ("Public".Equals(company, StringComparison.OrdinalIgnoreCase))
                company = "Public";

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
                    return Content("Not found");
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
                    backend.LogSourceFileFound(sourceFile);
                    Response.StatusCode = 404;
                    return Content("Source file not found");
                }

                backend.LogSourceFileFound(sourceFile);
                return Redirect(link);
            }
        }
    }
}
