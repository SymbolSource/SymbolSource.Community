using System.Collections.Generic;

namespace SymbolSource.Processing.Basic.Projects
{
    public class AddInfo : IAddInfo
    {      
        public IList<IBinaryInfo> Binaries { get; set; }
    }

    public class BinaryInfo : IBinaryInfo
    {
        public BinaryInfo(IAddInfo addInfo)
        {
            AddInfo = addInfo;
        }

        public string Name { get; set; }
        public string Type { get; set; }

        public string Hash { get; set; }
        public string SymbolHash { get; set; }

        public IFileInfo File { get; set; }
        public IAddInfo AddInfo { get; set; }
        public IDocumentationInfo DocumentationInfo { get; set; }
        public ISymbolInfo SymbolInfo { get; set; }
    }

    public class DocumentationInfo : IDocumentationInfo
    {
        public DocumentationInfo(IBinaryInfo binaryInfo)
        {
            BinaryInfo = binaryInfo;
        }

        public string Type { get; set; }
        public IBinaryInfo BinaryInfo { get; set; }
        public IFileInfo File { get; set; }
    }

    public class SymbolInfo : ISymbolInfo
    {
        public SymbolInfo(IBinaryInfo binaryInfo)
        {
            BinaryInfo = binaryInfo;
        }

        public string Type { get; set; }
        public string Hash { get; set; }

        public IFileInfo File { get; set; }
        public IBinaryInfo BinaryInfo { get; set; }
        public IList<ISourceInfo> SourceInfos { get; set; }
    }

    public class SourceInfo : ISourceInfo
    {
        public SourceInfo(ISymbolInfo symbolInfo)
        {
            SymbolInfo = symbolInfo;
        }

        public string OriginalPath { get; set; }
        public string KeyPath { get; set; }
        public ISymbolInfo SymbolInfo { get; set; }
        public IFileInfo ActualPath { get; set; }
        public string Md5Hash { get; set; }
    }
}
