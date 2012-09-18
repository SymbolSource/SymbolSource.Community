using System;
using System.Text;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using SymbolSource.Gateway.Core;
using SymbolSource.Gateway.NuGet.Core;
using SymbolSource.Server.Management.Client;

[assembly: PreApplicationStartMethod(typeof(ODataUserAuthorizationModule), "Register")]

namespace SymbolSource.Gateway.NuGet.Core
{
    public sealed class ODataUserAuthorizationModule : IHttpModule
    {
        void IHttpModule.Init(HttpApplication application)
        {
            application.PreRequestHandlerExecute += ApplicationOnPreRequestHandlerExecute;
        }

        void IHttpModule.Dispose()
        {
        }

        private void ApplicationOnPreRequestHandlerExecute(object sender, EventArgs eventArgs)
        {
            var application = (HttpApplication) sender;
            var wrapper = new HttpContextWrapper(application.Context);

            if (wrapper.Request.RequestContext.RouteData.RouteHandler is DynamicServiceRoute 
                /* && !string.IsNullOrEmpty((string) wrapper.Request.RequestContext.RouteData.Values["servicePath"]) */)
            {
                string company = (string)wrapper.Request.RequestContext.RouteData.Values["company"];
                string login = (string)wrapper.Request.RequestContext.RouteData.Values["login"];
                string key = (string)wrapper.Request.RequestContext.RouteData.Values["key"];
                string repository = (string)wrapper.Request.RequestContext.RouteData.Values["repository"];
                application.Context.Items.Add("Repository", new Repository {Company = company, Name = repository});
                var factory = ServiceLocator.Resolve<IGatewayBackendFactory<IPackageBackend>>();
                var manager = ServiceLocator.Resolve<INuGetGatewayManager>();
                var configurationFactory = ServiceLocator.Resolve<IGatewayConfigurationFactory>();

                if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(key))
                {
                    var caller = new Caller { Company = company, Name = login, KeyType = "VisualStudio", KeyValue = key };

                    if (factory.Validate(caller) != null)
                    {
                        application.Context.Items.Add("Caller", caller);
                        return;
                    }                        

                    application.Response.StatusCode = 403;
                    application.CompleteRequest();
                    return;
                }


                var auth = wrapper.Request.Headers["Authorization"];
                if (auth != null)
                {
                    var token = Encoding.ASCII.GetString(Convert.FromBase64String(auth.Split(' ')[1])).Split(':');
                    var caller = new Caller { Company = company, Name = token[0], KeyType = "Password", KeyValue = token[1] };

                    if (factory.Validate(caller) != null)
                    {
                        application.Context.Items.Add("Caller", caller);
                        return;
                    }                        
                }

                if (!manager.AuthenticateDownload(company, repository))
                {
                    var configuration = configurationFactory.Create(company);

                    var caller =  new Caller { Company = company, Name = configuration.PublicLogin, KeyType = "API", KeyValue = configuration.PublicPassword };
                    application.Context.Items.Add("Caller", caller);
                    return;
                }

                application.Response.StatusCode = 401;
                application.Response.AddHeader("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", company));
                application.CompleteRequest();

            }
        }

        public static void Register()
        {
            DynamicModuleUtility.RegisterModule(typeof(ODataUserAuthorizationModule));
        }
    }
}