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

        public IAddInfo Build(IDirectoryInfo directoryInfo)
        {
            var addInfo = new AddInfo();

            var items = GetAllFilePath(directoryInfo).ToArray();

            addInfo.Binaries = items
                .Where(f => f.Name.EndsWith("exe", StringComparison.InvariantCultureIgnoreCase) || f.Name.EndsWith("dll", StringComparison.InvariantCultureIgnoreCase))
                .Select(b => BuildBinaryInfo(addInfo, items, b))
                .ToArray();

            return addInfo;
        }                                    
 
        public IAddInfo Build(IDirectoryInfo directoryInfo, IEnumerable<IFileInfo> includeFiles)
        {
            var addInfo = new AddInfo();

            var items = GetAllFilePath(directoryInfo).ToArray();

            addInfo.Binaries = includeFiles
                .Select(b => BuildBinaryInfo(addInfo, items, b))
                .ToArray();

            return addInfo;
        }

        private IBinaryInfo BuildBinaryInfo(IAddInfo addInfo, IList<IFileInfo> fileInfos, IFileInfo binaryFileInfo)
        {
            var binaryInfo = new BinaryInfo(addInfo);

            binaryInfo.Name = Path.GetFileNameWithoutExtension(binaryFileInfo.Name);
            binaryInfo.Type = Path.GetExtension(binaryFileInfo.Name).Substring(1);
            binaryInfo.File = binaryFileInfo;

            using (var stream = binaryFileInfo.GetStream(FileMode.Open))
            {
                binaryInfo.Hash = binaryStoreManager.ReadBinaryHash(stream);
                stream.Seek(0, SeekOrigin.Begin);
                binaryInfo.SymbolHash = binaryStoreManager.ReadPdbHash(stream);
            }

            string symbolName = Path.ChangeExtension(binaryFileInfo.Name, "pdb");
            var symbolFileInfo = binaryFileInfo.ParentInfo.GetFile(symbolName);
            if (symbolFileInfo != null)
            {
                var symbolInfo = new SymbolInfo(binaryInfo);
                symbolInfo.Type = Path.GetExtension(symbolFileInfo.Name).Substring(1);
                symbolInfo.File = symbolFileInfo;

                using (var stream = symbolFileInfo.GetStream(FileMode.Open))
                {
                    symbolInfo.Hash = symbolStoreManager.ReadHash(stream);
                }

                symbolInfo.SourceInfos = sourceDiscover.FindSources(fileInfos, binaryInfo, symbolInfo).OrderBy(s => s.KeyPath).ToArray();
                binaryInfo.SymbolInfo = symbolInfo;
            }

            string documentationName = Path.ChangeExtension(binaryFileInfo.Name, "xml");
            var documentationFileInfo = binaryFileInfo.ParentInfo.GetFile(documentationName);
            if (documentationFileInfo != null)
            {
                var documentationInfo = new DocumentationInfo(binaryInfo);
                documentationInfo.Type = Path.GetExtension(documentationFileInfo.Name).Substring(1);
                documentationInfo.File = documentationFileInfo;
                binaryInfo.DocumentationInfo = documentationInfo;
            }

            return binaryInfo;
        }

        private static IEnumerable<IFileInfo> GetAllFilePath(IDirectoryInfo directoryInfo)
        {
            foreach (var file in directoryInfo.GetFiles())
                yield return file;

            foreach (var file in directoryInfo.GetDirectories().SelectMany<IDirectoryInfo, IFileInfo>(GetAllFilePath))
                yield return file;
        }
    }
}