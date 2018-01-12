﻿using System.Web.Mvc;
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
    public static class MicroKernel
    {
        private static IWindsorContainer _container;

        public static IWindsorContainer Container
        {
            get { return _container ?? (_container = new WindsorContainer()); }
        }

        public static IKernel Install()
        {
            var container = Container;
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
            container.Install(new ProcessingBasicInstaller());

            RegisterManagers(container);

            container.Register(
                AllTypes.FromAssembly(typeof(Gateway.NuGet.Core.AttributeRouting).Assembly)
                    .BasedOn<IController>()
                    .LifestyleTransient()
                );

            container.Register(
                AllTypes.FromAssembly(typeof(Gateway.WinDbg.Core.AttributeRouting).Assembly)
                    .BasedOn<IController>()
                    .LifestyleTransient()
                );

            container.Register(
               AllTypes.FromThisAssembly()
                   .BasedOn<IController>()
                   .LifestyleTransient()
               );

            ControllerBuilder.Current.SetControllerFactory(new MCControllerFactory(container.Kernel));
            ServiceLocator.Locator = new SimpleServiceLocator(container.Resolve);

            return container.Kernel;
        }

        private static void RegisterManagers(IWindsorContainer container)
        {
            container
               
                .Register(
                    Component.For<IGatewayBackendFactory<IWinDbgBackend>, IGatewayBackendFactory<IPackageBackend>>()
                       .ImplementedBy<BasicBackendFactory>()
                       .OnlyNewServices()
                   )
    
                .Register(
                    Component.For<INuGetGatewayManager>()
                        .ImplementedBy<NuGetGatewayManager>()
                        .OnlyNewServices()
                    )
    
                .Register(
                    Component.For<INuGetGatewayVersionExtractor, IGatewayVersionExtractor>()
                        .ImplementedBy<NuGetGatewayVersionExtractor>()
                        .OnlyNewServices()
                    )
    
                .Register(
                    Component.For<IBasicBackendConfiguration>()
                        .ImplementedBy<BasicBackendConfiguration>()
                        .OnlyNewServices()
                    )
    
                .Register(
                    Component.For<IGatewayConfigurationFactory>()
                        .ImplementedBy<AppSettingsConfigurationFactory>()
                        .OnlyNewServices()
                    );
        }
    }
}
