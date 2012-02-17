using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SymbolSource.Server.Basic.Host
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string GetAbsoluteUri(string relativeUri)
        {
            Uri baseUri;

            if (Uri.TryCreate(ConfigurationManager.AppSettings["BaseUrl"], UriKind.Absolute, out baseUri))
                return new Uri(baseUri, relativeUri).AbsoluteUri;

            return Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "") + '/' + relativeUri.TrimStart('/');
        }
    }
}