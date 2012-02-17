using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel;

namespace SymbolSource.Server.Basic
{
    [DebuggerNonUserCode]
    public class MCControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel kernel;

        public MCControllerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", requestContext.HttpContext.Request.Path));

            var controller = (IController)kernel.Resolve(controllerType);
            return controller;
        }

        public override void ReleaseController(IController controller)
        {
            var disposable = controller as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            kernel.ReleaseComponent(controller);
        }
    }
}