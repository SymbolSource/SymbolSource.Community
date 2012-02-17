using System;
using System.Configuration;
using Castle.Components.DictionaryAdapter;
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

            container.Register(
                Component.For<IPdbStoreConfig>()
                    .Instance(new DictionaryAdapterFactory().GetAdapter<IPdbStoreConfig>(ConfigurationManager.AppSettings))
                );

            container.Register(
              Component.For<IPdbStoreManager>()
                  .Forward<ISymbolStoreManager>()
                  .ImplementedBy<PdbStoreManager>()
              );

            container.Register(
               Component.For<IFileCompressor>()
                   .ImplementedBy<FileCompressor>()
               );
        }
    }
}