using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using SymbolSource.Gateway.Core;
using SymbolSource.Processing.Basic;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.WinDbg.Core
{
    public class PdbController : Controller
    {
        private readonly IGatewayBackendFactory<IWinDbgBackend> factory;
        private readonly IPdbStoreManager pdbStoreManager;
        private readonly IFileCompressor fileCompressor;

        public PdbController(
            IGatewayBackendFactory<IWinDbgBackend> factory,
            IFileCompressor fileCompressor,
            IPdbStoreManager pdbStoreManager
            )
        {
            this.factory = factory;
            this.pdbStoreManager = pdbStoreManager;
            this.fileCompressor = fileCompressor;
        }

        public ActionResult Index(string company, string login, string password, string name, string hash, string name1)
        {
            if ("Public".Equals(company, StringComparison.OrdinalIgnoreCase))
                company = "Public";

            if (!name1.EndsWith(".pd_", StringComparison.InvariantCultureIgnoreCase))
            {
                Response.StatusCode = 404;
                return Content("Supported only compresioned files (.pd_)");
            }

            //Ktoś ma małą literką numer kompilacji
            hash = hash.ToUpper();

            string imageName = Path.GetFileNameWithoutExtension(name);

            using (var backend = factory.Create(company, login, "VisualStudio", password))
            {
                var imageFile = backend.GetImageFile(imageName, hash);

                if (imageFile != null)
                    return HandleFound(backend, company, login, password, imageFile);
            }

            if (company != "Public")
            {
                var configuration = new ConfigurationWrapper("Public");

                using (var backend = factory.Create("Public", configuration.PublicLogin, "VisualStudio", configuration.PublicPassword))
                {
                    var imageFile = backend.GetImageFile(imageName, hash);
                    
                    if (imageFile != null)
                        return HandleFound(backend, company, login, password, imageFile);
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

        private ActionResult HandleFound(IWinDbgBackend backend, string company, string login, string password, ImageFile imageFile)
        {
            backend.LogImageFileFound(imageFile);

            var routeData = new
                                {
                                    company,
                                    login,
                                    password,
                                    imageName = imageFile.Name,
                                    pdbHash = imageFile.SymbolHash,
                                    sourcePath = "XVAR2X",
                                    computerName = "XCNX",
                                    computerUser = "XUNX",
                                };

            var url = Request.Url.GetLeftPart(UriPartial.Authority) + Url.RouteUrl("Source", routeData);
            return PdbFile(backend, imageFile, url);
        }

        private ActionResult PdbFile(IWinDbgBackend backend, ImageFile imageFile, string sourcePath)
        {
            var directoryPathTemp = Path.Combine(Path.GetTempPath(), "hss-temp-" + Path.GetRandomFileName());
            Directory.CreateDirectory(directoryPathTemp);

            var link = backend.GetSymbolFileLink(ref imageFile);
            var tempPath = Path.Combine(directoryPathTemp, imageFile.Name + ".pdb");
            var temp2Path = Path.Combine(directoryPathTemp, imageFile.Name + ".pd_");

            try
            {
                DownloadFile(link, tempPath);

                var pdbstr = new PdbSrcSrvSection();

                pdbstr.Ini.Add("VERSION", "2");
                pdbstr.Ini.Add("INDEXVERSION", "2");
                //TODO: jaka jest idea tych podmian, zamiast od razu wkleić %cośtam%?
                pdbstr.Variables.Add("SRCSRVTRG", sourcePath.Replace("XVAR2X", "%var2%").Replace("XCNX", "%CN%").Replace("XUNX", "%UN%"));
                pdbstr.Variables.Add("SRCSRVCMD", string.Empty);
                pdbstr.Variables.Add("UN", "%USERNAME%");
                pdbstr.Variables.Add("CN", "%COMPUTERNAME%");
                pdbstr.Variables.Add("SRCSRVVERCTRL", "http");
                pdbstr.Variables.Add("SRCSRVERRVAR", "var2");
                pdbstr.Ini.Add("VERCTRL", "http");

                var sources = backend.GetSourceFileList(ref imageFile);

                foreach (var source in sources)
                    pdbstr.Sources.Add(new[] { source.OriginalPath, source.Path, source.Hash });

                pdbStoreManager.WriteSrcSrv(tempPath, pdbstr);
                fileCompressor.Compress(tempPath, temp2Path);

                return File(System.IO.File.ReadAllBytes(temp2Path), "application/octet-stream");
            }
            finally
            {
                System.IO.File.Delete(tempPath);
                System.IO.File.Delete(temp2Path);
                Directory.Delete(directoryPathTemp);
            }
        }

        private static void DownloadFile(string link, string destination)
        {
            try
            {
                var request = WebRequest.Create(link);
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var outputStream = System.IO.File.OpenWrite(destination))
                {
                    responseStream.CopyTo(outputStream);

                    if (outputStream.Length < 1000)
                        throw new WebException(string.Format("File is too small ({0} length)", outputStream.Length));
                }
            }
            catch(WebException e)
            {
                throw new WebException(link, e);
            }
            
        }
    }
}
