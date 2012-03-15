using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymbolSource.Processing.Basic.Projects
{
    public class AddInfoBuilder : IAddInfoBuilder
    {
        private readonly IBinaryStoreManager binaryStoreManager;
        private readonly ISymbolStoreManager symbolStoreManager;
        private readonly SourceDiscover sourceDiscover;

        public AddInfoBuilder(IBinaryStoreManager binaryStoreManager, ISymbolStoreManager symbolStoreManager, SourceDiscover sourceDiscover)
        {
            this.binaryStoreManager = binaryStoreManager;
            this.symbolStoreManager = symbolStoreManager;
            this.sourceDiscover = sourceDiscover;
        }

        public IAddInfo Build(IPackageFile directoryInfo)
        {
            var addInfo = new AddInfo();

            var items = directoryInfo.Entries.ToArray();

            addInfo.Binaries = items
                .Where(f => f.FullPath.EndsWith("exe", StringComparison.InvariantCultureIgnoreCase) || f.FullPath.EndsWith("dll", StringComparison.InvariantCultureIgnoreCase))
                .Select(b => BuildBinaryInfo(addInfo, items, b))
                .ToArray();

            return addInfo;
        }

        public IAddInfo Build(IPackageFile directoryInfo, IEnumerable<IPackageEntry> includeFiles)
        {
            var addInfo = new AddInfo();

            var items = directoryInfo.Entries.ToArray();

            addInfo.Binaries = includeFiles
                .Select(b => BuildBinaryInfo(addInfo, items, b))
                .ToArray();

            return addInfo;
        }

        private IBinaryInfo BuildBinaryInfo(IAddInfo addInfo, IList<IPackageEntry> fileInfos, IPackageEntry binaryFileInfo)
        {
            var binaryInfo = new BinaryInfo(addInfo);

            binaryInfo.Name = Path.GetFileNameWithoutExtension(binaryFileInfo.FullPath);
            binaryInfo.Type = Path.GetExtension(binaryFileInfo.FullPath).Substring(1);
            binaryInfo.File = binaryFileInfo;

            using (var stream = binaryFileInfo.Stream)
            {
                binaryInfo.Hash = binaryStoreManager.ReadBinaryHash(stream);
                stream.Seek(0, SeekOrigin.Begin);
                binaryInfo.SymbolHash = binaryStoreManager.ReadPdbHash(stream);
            }

            string symbolName = Path.ChangeExtension(binaryFileInfo.FullPath, "pdb");
            var symbolFileInfo = fileInfos.SingleOrDefault(s => s.FullPath == symbolName);
            if (symbolFileInfo != null)
            {
                var symbolInfo = new SymbolInfo(binaryInfo);
                symbolInfo.Type = Path.GetExtension(symbolFileInfo.FullPath).Substring(1);
                symbolInfo.File = symbolFileInfo;

                using (var stream = symbolFileInfo.Stream)
                {
                    symbolInfo.Hash = symbolStoreManager.ReadHash(stream);
                }

                symbolInfo.SourceInfos = sourceDiscover.FindSources(fileInfos, binaryInfo, symbolInfo).OrderBy(s => s.KeyPath).ToArray();
                binaryInfo.SymbolInfo = symbolInfo;
            }

            string documentationName = Path.ChangeExtension(binaryFileInfo.FullPath, "xml");
            var documentationFileInfo = fileInfos.SingleOrDefault(s => s.FullPath == documentationName);
            if (documentationFileInfo != null)
            {
                var documentationInfo = new DocumentationInfo(binaryInfo);
                documentationInfo.Type = Path.GetExtension(documentationFileInfo.FullPath).Substring(1);
                documentationInfo.File = documentationFileInfo;
                binaryInfo.DocumentationInfo = documentationInfo;
            }

            return binaryInfo;
        }
    }
}