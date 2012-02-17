using System;
using SymbolSource.Gateway.Core;
using SymbolSource.Server.Management.Client;

namespace SymbolSource.Gateway.WinDbg.Core
{
    public interface IWinDbgBackend : IGatewayBackend
    {
        ImageFile GetImageFile(string name, string symbolHash);
        SourceFile[] GetSourceFileList(ref ImageFile imageFile);

        string GetSourceFileLink(ref SourceFile sourceFile);
        string GetImageFileLink(ref ImageFile imageFile);
        string GetSymbolFileLink(ref ImageFile imageFile);

        void LogImageFileFound(ImageFile imageFile);
        void LogImageFileNotFound(string imageFileName, string symbolHash);
        void LogSourceFileFound(SourceFile sourceFile);
    }
}