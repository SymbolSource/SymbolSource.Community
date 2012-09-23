using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Routing;

namespace SymbolSource.Gateway.NuGet.Core
{
    //Thanks to:
    //http://blog.maartenballiauw.be/post/2011/11/08/Rewriting-WCF-OData-Services-base-URL-with-load-balancing-reverse-proxy.aspx
    //http://blog.maartenballiauw.be/post/2011/05/09/Using-dynamic-WCF-service-routes.aspx
    //http://blogs.msdn.com/b/astoriateam/archive/2010/07/21/odata-and-authentication-part-6-custom-basic-authentication.aspx

    public class RewriteBaseUrlMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (WebOperationContext.Current != null && WebOperationContext.Current.IncomingRequest.UriTemplateMatch != null)
            {
                var baseUriBuilder = new UriBuilder(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri);
                var requestUriBuilder = new UriBuilder(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri);

                var routeData = DynamicServiceRoute.GetCurrentRouteData();
                var route = routeData.Route as Route;
                if (route != null)
                {
                    string servicePath = route.Url;
                    foreach (var routeValue in routeData.Values)
                        if (routeValue.Value != null)
                            servicePath = servicePath.Replace("{" + routeValue.Key + "}", routeValue.Value.ToString());

                    servicePath = servicePath.Replace("{*servicePath}", string.Empty);

                    if (!servicePath.StartsWith("/"))
                        servicePath = "/" + servicePath;

                    if (!servicePath.EndsWith("/"))
                        servicePath = servicePath + "/";

                    requestUriBuilder.Path = requestUriBuilder.Path.Replace(baseUriBuilder.Path, servicePath);
                    requestUriBuilder.Host = baseUriBuilder.Host;
                    baseUriBuilder.Path = servicePath;
                }

                OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRootUri"] = baseUriBuilder.Uri;
                OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRequestUri"] = requestUriBuilder.Uri;
            }


            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }
    }

    public class RewriteBaseUrlBehavior : Attribute, IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new RewriteBaseUrlMessageInspector());
        }
    }

    public class DynamicServiceRoute : RouteBase, IRouteHandler
    {
        private readonly string virtualPath;
        private readonly ServiceRoute innerServiceRoute;
        private readonly Route innerRoute;

        public static RouteData GetCurrentRouteData()
        {
            if (HttpContext.Current != null)
            {
                var wrapper = new HttpContextWrapper(HttpContext.Current);
                return wrapper.Request.RequestContext.RouteData;
            }
            return null;
        }

        public DynamicServiceRoute(string pathPrefix, object defaults, string[] namespaces, ServiceHostFactoryBase serviceHostFactory, Type serviceType)
        {
            if (pathPrefix.IndexOf("{*", StringComparison.Ordinal) >= 0)
                throw new ArgumentException("Path prefix can not include catch-all route parameters.", "pathPrefix");

            if (!pathPrefix.EndsWith("/"))
                pathPrefix += "/";

            pathPrefix += "{*servicePath}";

            virtualPath = serviceType.FullName + "-" + Guid.NewGuid().ToString() + "/";
            innerServiceRoute = new ServiceRoute(virtualPath, serviceHostFactory, serviceType);
            
            innerRoute = new Route(pathPrefix, new RouteValueDictionary(defaults), this)
                             {
                                 DataTokens = new RouteValueDictionary()
                             };

            if ((namespaces != null) && (namespaces.Length > 0))
            {
                innerRoute.DataTokens["Namespaces"] = namespaces;
            }
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            return innerRoute.GetRouteData(httpContext);
        }

        public override VirtualPathData GetVirtualPath(System.Web.Routing.RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }

        public IHttpHandler GetHttpHandler(System.Web.Routing.RequestContext requestContext)
        {
            requestContext.HttpContext.RewritePath("~/" + virtualPath + requestContext.RouteData.Values["servicePath"], true);
            return innerServiceRoute.RouteHandler.GetHttpHandler(requestContext);
        }

        public Route InnerRoute
        {
            get { return innerRoute; }
        }
    }

}