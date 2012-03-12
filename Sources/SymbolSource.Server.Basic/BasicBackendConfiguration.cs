using System;
using System.Web;

namespace SymbolSource.Server.Basic
{
    public class BasicBackendConfiguration : IBasicBackendConfiguration
    {
        public string DataPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Data"); }
        }

        public string IndexPath
        {
            get { return HttpContext.Current.Server.MapPath("~/Index"); }
        }

        public string RemotePath
        {
            get
            {
                return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "Data";
            }
        }
    }
}