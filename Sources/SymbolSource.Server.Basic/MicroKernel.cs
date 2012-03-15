using System.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Castle.Components.DictionaryAdapter;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using SymbolSource.Gateway.Core;
using SymbolSource.Gateway.NuGet.Core;
using SymbolSource.Gateway.OpenWrap.Core;
using SymbolSource.Gateway.WinDbg.Core;
using SymbolSource.Processing.Basic;

namespace SymbolSource.Server.Basic
{
    public class MicroKernel
    {
        public static IKernel Install()
        {
            var container = new WindsorContainer();
           container.Install(new ProcessingBasicInstaller());           

            RegisterManagers(container);

            container.Register(
                AllTypes.FromAssembly(typeof(Gateway.NuGet.Core.AttributeRouting).Assembly)
                    .BasedOn<IController>()
                    .Configure(cr => cr.LifeStyle.Transient)
                );

            container.Register(
                AllTypes.FromAssembly(typeof(Gateway.OpenWrap.Core.AttributeRouting).Assembly)
                    .BasedOn<IController>()
                    .Configure(cr => cr.LifeStyle.Transient)
                );

            container.Register(
                AllTypes.FromAssembly(typeof(Gateway.WinDbg.Core.AttributeRouting).Assembly)
                    .BasedOn<IController>()
                    .Configure(cr => cr.LifeStyle.Transient)
                );
            
            ControllerBuilder.Current.SetControllerFactory(new MCControllerFactory(container.Kernel));

            return container.Kernel;
        }

        private static void RegisterManagers(IWindsorContainer kernel)
        {
            kernel.Register(
               Component.For<IGatewayBackendFactory<IWinDbgBackend>, IGatewayBackendFactory<IPackageBackend>>()
                   .ImplementedBy<BasicBackendFactory>()
               );

            kernel.Register(
                Component.For<INuGetGatewayManager>()
                    .ImplementedBy<NuGetGatewayManager>()
                );

            kernel.Register(
                Component.For<IOpenWrapGatewayManager>()
                    .ImplementedBy<OpenWrapGatewayManager>()
                );

            kernel.Register(
                Component.For<IBasicBackendConfiguration>()
                    .ImplementedBy<BasicBackendConfiguration>()
                );

            kernel.Register(
                Component.For<IGatewayConfigurationFactory>()
                    .ImplementedBy<AppSettingsConfigurationFactory>()
                );
        }     
    }

}
