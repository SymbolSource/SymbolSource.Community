using System.Collections.Generic;

namespace SymbolSource.Processing.Basic.Projects
{
    public class AddInfoBaseVisitor : IAddInfoVisitor
    {
        public virtual IAddInfo Visit(IAddInfo addInfo)
        {
            Visit(addInfo.Binaries);
            return addInfo;
        }

        public virtual IList<IBinaryInfo> Visit(IList<IBinaryInfo> binaryInfos)
        {
            foreach (var binaryInfo in binaryInfos)
                Visit(binaryInfo);
            return binaryInfos;
        }

        public virtual IBinaryInfo Visit(IBinaryInfo binaryInfo)
        {
            if (binaryInfo.SymbolInfo!=null)
                Visit(binaryInfo.SymbolInfo);
            if (binaryInfo.DocumentationInfo != null)
                Visit(binaryInfo.DocumentationInfo);
            return binaryInfo;
        }

        public virtual IDocumentationInfo Visit(IDocumentationInfo documentInfo)
        {
            return documentInfo;
        }

        public virtual ISymbolInfo Visit(ISymbolInfo symbolInfo)
        {
            Visit(symbolInfo.SourceInfos);
            return symbolInfo;
        }

        public virtual IList<ISourceInfo> Visit(IList<ISourceInfo> sourceInfos)
        {
            foreach (var sourceInfo in sourceInfos)
                Visit(sourceInfo);
            return sourceInfos;
        }

        public virtual ISourceInfo Visit(ISourceInfo sourceInfo)
        {
            return sourceInfo;
        }      
    }
}
