using System.Web.Mvc;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using SymbolSource.Gateway.Core;
using SymbolSource.Gateway.NuGet.Core;
using SymbolSource.Gateway.WinDbg.Core;
using SymbolSource.Processing.Basic;

namespace SymbolSource.Server.Basic
{
    public class MicroKernel
    {
        public static IKernel Install()
        {
            var container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
            container.Install(new ProcessingBasicInstaller());

            RegisterManagers(container);

            container.Register(
                Types.FromAssembly(typeof(Gateway.NuGet.Core.AttributeRouting).Assembly)
                    .BasedOn<IController>()
                    .LifestyleTransient()
                );

            container.Register(
                Types.FromAssembly(typeof(Gateway.WinDbg.Core.AttributeRouting).Assembly)
                    .BasedOn<IController>()
                    .LifestyleTransient()
                );

            container.Register(
               Types.FromThisAssembly()
                   .BasedOn<IController>()
                   .LifestyleTransient()
               );

            ControllerBuilder.Current.SetControllerFactory(new MCControllerFactory(container.Kernel));
            ServiceLocator.Locator = new SimpleServiceLocator(container.Resolve);

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
                Component.For<INuGetGatewayVersionExtractor, IGatewayVersionExtractor>()
                    .ImplementedBy<NuGetGatewayVersionExtractor>()
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
