using System;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public string GetSymbolFileLink(ref ImageFile imageFile)
        {
            return string.Format("{0}/Images/{1}/{3}/{1}.pdb", configuration.RemotePath, imageFile.Name, imageFile.BinaryType, imageFile.SymbolHash);
        }

        public string GetImageFileLink(ref ImageFile imageFile)
        {
            return string.Format("{0}/Images/{1}/{3}/{1}.{2}", configuration.RemotePath, imageFile.Name, imageFile.BinaryType, imageFile.SymbolHash);
        }

        public string GetSourceFileLink(ref SourceFile sourceFile)
        {
            return string.Format("{0}/Images/{1}/{3}/Sources/", configuration.RemotePath, sourceFile.ImageName, sourceFile.Hash, sourceFile.Path);
        }

        public string GetPackageLink(ref Version version, string contentType)
        {
            throw new NotImplementedException();
        }
    }
}