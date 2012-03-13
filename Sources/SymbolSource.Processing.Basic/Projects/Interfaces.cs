using System.Collections.Generic;
using System.IO;

namespace SymbolSource.Processing.Basic.Projects
{
    public interface  IPackageFile
    {
        IEnumerable<IPackageEntry> Entries { get; }
    }

    public interface IPackageEntry
    {
        string FullPath { get; }
        Stream Stream { get; }
    }

    public interface IAddInfoBuilder
    {
        IAddInfo Build(IPackageFile directoryInfo);
        IAddInfo Build(IPackageFile directoryInfo, IEnumerable<IPackageEntry> includeFiles);
    }

    public interface IAddInfo
    {
        IList<IBinaryInfo> Binaries { get; }
    }

    public interface IBinaryInfo
    {
        string Name { get; }
        string Type { get; }

        string Hash { get; }
        string SymbolHash { get; }

        IAddInfo AddInfo { get; set; }

        IPackageEntry File { get; }
        IDocumentationInfo DocumentationInfo { get; }
        ISymbolInfo SymbolInfo { get; }
    }

    public interface IDocumentationInfo
    {
        string Type { get; }

        IBinaryInfo BinaryInfo { get; set; }

        IPackageEntry File { get; }
    }

    public interface ISymbolInfo
    {
        string Type { get; }
        string Hash { get; }

        IBinaryInfo BinaryInfo { get; set; }

        IPackageEntry File { get; }
        IList<ISourceInfo> SourceInfos { get; }
    }

    public interface ISourceInfo
    {

        string OriginalPath { get; }
        string KeyPath { get; }
        string Md5Hash { get; }

        ISymbolInfo SymbolInfo { get; set; }

        IPackageEntry ActualPath { get; }
    }

    public interface IAddInfoVisitor
    {
        IAddInfo Visit(IAddInfo addInfo);
        IList<IBinaryInfo> Visit(IList<IBinaryInfo> binaryInfos);
        IBinaryInfo Visit(IBinaryInfo binaryInfo);
        IDocumentationInfo Visit(IDocumentationInfo documentInfo);
        ISymbolInfo Visit(ISymbolInfo symbolInfo);
        IList<ISourceInfo> Visit(IList<ISourceInfo> sourceInfos);
        ISourceInfo Visit(ISourceInfo sourceInfo);
    }

    public interface ITransformation
    {
        string EncodePath(string path);
        string DecodePath(string path);
        Stream DecodeContent(Stream stream);
        Stream EncodeContent(Stream stream);
    }
}
