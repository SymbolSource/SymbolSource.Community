using System;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public void CreateJob(byte[] data, PackageProject metadata)
        {
            throw new NotImplementedException();
        }

        public void PushPackage(ref Version version, byte[] data, PackageProject metadata)
        {
            throw new NotImplementedException();
        }
    }
}