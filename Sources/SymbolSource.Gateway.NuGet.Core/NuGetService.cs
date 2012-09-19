using System;
using System.Net;

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

            var response = GetResponse(request);
            
            switch (response)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NotFound:
                    return true;

                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Forbidden:
                    return false;

                default:
                    throw new Exception(response.ToString());
            }
           
        }

        private HttpStatusCode GetResponse(WebRequest request)
        {
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode;
                }
            }
            catch (WebException exception)
            {
                return ((HttpWebResponse)exception.Response).StatusCode;
            }
        }
    }
}
