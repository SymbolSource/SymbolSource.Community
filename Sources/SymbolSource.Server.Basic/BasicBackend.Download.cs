using System;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public string GetSymbolFileLink(ref ImageFile imageFile)
        {
            throw new NotImplementedException();
        }

        public string GetImageFileLink(ref ImageFile imageFile)
        {
            throw new NotImplementedException();
        }

        public string GetSourceFileLink(ref SourceFile sourceFile)
        {
            throw new NotImplementedException();
        }

        public string GetPackageLink(ref Version version, string contentType)
        {
            throw new NotImplementedException();
        }
    }
}