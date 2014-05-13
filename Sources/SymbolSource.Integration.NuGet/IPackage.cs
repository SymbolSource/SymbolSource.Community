using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymbolSource.Integration.NuGet
{
    public interface IPackage
    {
        string FileName { get; }
        IEnumerable<IPackageFile> GetFiles();
    }

    public interface IPackageFile
    {
        string Path { get; }
        Stream GetStream();
    }
}
