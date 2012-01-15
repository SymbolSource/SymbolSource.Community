using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SymbolSource.Processing.Basic.Projects;

namespace SymbolSource.Processing.Basic
{
    public class ProcessingBasicInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IBinaryStoreManager>()
                    .ImplementedBy<BinaryStoreManager>()
                );

            container.Register(
                Component.For<SourceDiscover>()
                    .ImplementedBy<SourceDiscover>()
                );

            container.Register(
                Component.For<IAddInfoBuilder>()
                    .ImplementedBy<AddInfoBuilder>()
                );

            container.Register(
                Component.For<ISourceStoreManager>()
                    .ImplementedBy<SourceStoreManager>()
                );



            container.Register(
                Component.For<ISymbolStoreManager>()
                    .ImplementedBy<SymbolStoreManager>()
                );

            container.Register(
                Component.For<ISourceExtractor>()
                    .ImplementedBy<ManagedSourceExtractor>()
                );

        }
    }
}