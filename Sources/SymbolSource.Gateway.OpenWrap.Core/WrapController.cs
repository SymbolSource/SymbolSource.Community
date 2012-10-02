using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using AttributeRouting.Web.Mvc;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.OpenWrap.Core
{
    public class WrapController : Controller
    {
        private readonly IGatewayBackendFactory<IPackageBackend> factory;
        private readonly IGatewayManager manager;

        public WrapController(IGatewayBackendFactory<IPackageBackend> factory, IOpenWrapGatewayManager manager)
        {
            this.factory = factory;
            this.manager = manager;           
        }

        private Caller Authenticate(string company, bool require)
        {
            var auth = Request.Headers["Authorization"];

            if (auth != null)
            {
                var caller = factory.DigestValidateResponse(company, Request.HttpMethod, auth);

                if (caller != null)
                    return caller;
            }

            if (!require)
            {
                var configuration = new AppSettingsConfiguration(company);

                return new Caller
                           {
                               Company = company,
                               Name = configuration.PublicLogin,
                               KeyType = "API",
                               KeyValue = configuration.PublicPassword
                           };
            }

            Response.StatusCode = 401;
            Response.AddHeader("WWW-Authenticate", factory.DigestGenerateRequest(company));
            return null;
        }

        [GET("{company}/{repository}/index.wraplist")]
        public ActionResult Index(string company, string repository)
        {
            if (Request.HttpMethod == "HEAD")
                return null;

            var caller = Authenticate(company, manager.AuthenticateDownload(company, repository));

            if (caller == null)
                return null;

            var versions = manager.Index(caller, company, repository);

            return Content(
                new XDocument(
                    new XElement("wraplist",
                                 new XObject[]
                                     {
                                         new XAttribute("read-only", false),
                                         new XElement("link",
                                                      new XAttribute("rel", "publish"),
                                                      new XAttribute("href", "upload"))
                                     }.Concat(versions
                                                  .Where(version => IsCompatibleVersion(version.Name))
                                                  .Select(
                                                      version =>
                                                      new XElement("wrap",
                                                                   new XAttribute("name", version.Project),
                                                                   new XAttribute("version", version.Name),
                                                                   new XElement("link",
                                                                                new XAttribute("rel", "package"),
                                                                                new XAttribute("href", string.Format("download/{0}/{1}", version.Project, version.Name))))
                                                  )))).ToString(),
                "application/vnd.openwrap.list.xml");
        }

        private bool IsCompatibleVersion(string name)
        {
            try
            {
                new System.Version(name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [GET("{company}/{repository}/download/{project}/{version}")]
        public ActionResult Download(string company, string repository, string project, string version)
        {
            var caller = Authenticate(company, manager.AuthenticateDownload(company, repository));

            if (caller == null)
                return null;

            var contentType = Request.ContentType;

            if (contentType == "")
                contentType = null;

            return Redirect(manager.Download(caller, company, repository, project, version, contentType));
        }

        [POST("{company}/{repository}/upload")]
        public ActionResult Upload(string company, string repository)
        {
            var caller = Authenticate(company, manager.AuthenticateUpload(company, repository));

            if (caller == null)
                return null;

            manager.Upload(caller, Request.InputStream, company, repository);
            return null;
        }
    }
}
