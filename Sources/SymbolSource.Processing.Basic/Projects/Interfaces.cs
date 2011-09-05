using System;
using System.Collections.Generic;
using System.IO;

namespace SymbolSource.Processing.Basic.Projects
{
    //Info o plikach
    public interface IInfo : IDisposable
    {
        IDirectoryInfo ParentInfo { get; }
        string FullPath { get; }
        string Name { get; }
    }

    public interface IFileInfo : IInfo
    {
        Stream GetStream(FileMode fileMode);
        byte[] ReadAllBytes();
    }

    public interface IDirectoryInfo : IInfo
    {
        IEnumerable<IFileInfo> GetFiles();
        IEnumerable<IDirectoryInfo> GetDirectories();
        IFileInfo GetFile(params string[] name);
        IDirectoryInfo GetDirectory(params string[] name);
    }

    //Binarki itp

    public interface IAddInfoBuilder
    {
        IAddInfo Build(IDirectoryInfo directoryInfo);
        IAddInfo Build(IDirectoryInfo directoryInfo, IEnumerable<IFileInfo> includeFiles);
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

        IFileInfo File { get; }
        IDocumentationInfo DocumentationInfo { get; }
        ISymbolInfo SymbolInfo { get; }
    }

    public interface IDocumentationInfo
    {
        string Type { get; }

        IBinaryInfo BinaryInfo { get; set; }

        IFileInfo File { get; }
    }

    public interface ISymbolInfo
    {
        string Type { get; }
        string Hash { get; }

        IBinaryInfo BinaryInfo { get; set; }

        IFileInfo File { get; }
        IList<ISourceInfo> SourceInfos { get; }
    }

    public interface ISourceInfo
    {

        string OriginalPath { get; }
        string KeyPath { get; }
        string Md5Hash { get; }

        ISymbolInfo SymbolInfo { get; set; }

        IFileInfo ActualPath { get; }
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
