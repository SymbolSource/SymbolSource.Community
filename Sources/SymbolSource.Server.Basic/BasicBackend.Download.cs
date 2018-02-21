﻿using System;
using System.IO;
using SymbolSource.Server.Management.Client.WebService;
using Version = SymbolSource.Server.Management.Client.WebService.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public string GetSymbolFileLink(ref ImageFile imageFile)
        {
            string path = Path.Combine(GetPathFromImageFile(imageFile), imageFile.Name + ".pdb");
            return configuration.RemotePath + '/' + path.Replace(Path.DirectorySeparatorChar, '/');
        }

        public string GetImageFileLink(ref ImageFile imageFile)
        {
            throw new NotImplementedException();
        }

        public string GetSourceFileLink(ref SourceFile sourceFile)
        {
            string path = GetPathFromSourceFile(sourceFile);
            return configuration.RemotePath + '/' + path.Replace(Path.DirectorySeparatorChar, '/');
        }

        public string GetPackageLink(ref Version version, string contentType)
        {
            return configuration.RemotePath + '/' + GetPackagePathFromVersion(version, null).Replace(Path.DirectorySeparatorChar, '/');
        }
    }
}