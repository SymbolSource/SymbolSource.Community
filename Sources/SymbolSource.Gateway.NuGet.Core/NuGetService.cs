using System;
using System.Net;
using System.Web;
using Elmah;

namespace SymbolSource.Gateway.NuGet.Core
{
    public class NuGetService
    {
        private readonly string path;

        public NuGetService(string path)
        {
            this.path = path;
        }

        public bool CheckPermission(string key, string project)
        {
            var request = WebRequest.Create(string.Format("{0}/api/v2/verifykey/{1}", path, project));
            request.Headers["X-NuGet-ApiKey"] = key;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                        case HttpStatusCode.NotFound:
                            return true;

                        case HttpStatusCode.Forbidden:
                            return false;

                        default:
                            throw new Exception(response.StatusDescription);
                    }
                }
            }
            catch (Exception exception)
            {
                //TODO tutaj chyba nie powinno być odwołań do System.Web
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(exception, HttpContext.Current));
                return false;
            }
        }
    }
}
