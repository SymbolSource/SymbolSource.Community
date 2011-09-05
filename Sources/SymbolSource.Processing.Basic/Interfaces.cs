using System.Collections.Generic;
using System.IO;

namespace SymbolSource.Processing.Basic
{
    public interface IBinaryStoreManager
    {
        string ReadPdbHash(string binaryFilePath);
        string ReadBinaryHash(string binaryFilePath);
        string ReadPdbHash(Stream stream);
        string ReadBinaryHash(Stream stream);
    }


    public interface IFileCompressor
    {
        void Compress(string source, string destination);
        //Stream Compress(string originalFileName, Stream inputStream);
    }

    public interface ISymbolStoreManager
    {
        string ReadHash(string pdbFilePath);
        string ReadHash(Stream stream);
        void WriteHash(string pdbFilePath, string hash);
    }

    public interface ISourceExtractor
    {
        IList<string> ReadSources(string peFilePath, string pdbFilePath);
        IList<string> ReadSources(Stream peStream, Stream pdbStream);
    }

    public interface ISourceStoreManager
    {
        string ReadHash(Stream stream);
    }
}