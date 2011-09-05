using System.Collections.Generic;
using System.Linq;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public class TransformingDirectoryWrapper : TransformingWrapper<IDirectoryInfo>, IDirectoryInfo
    {
        public TransformingDirectoryWrapper(IDirectoryInfo info, ITransformation transformation)
            : base(info, transformation)
        {
        }

        private string[] EncodePath(params string[] name)
        {
            return name.Select(part => transformation.EncodePath(part)).ToArray();
        }

        public IEnumerable<IFileInfo> GetFiles()
        {
            return info.GetFiles().Select<IFileInfo, IFileInfo>(WrapFile);
        }

        public IEnumerable<IDirectoryInfo> GetDirectories()
        {
            return info.GetDirectories().Select<IDirectoryInfo, IDirectoryInfo>(WrapDirectory);
        }

        public IFileInfo GetFile(params string[] name)
        {
            return WrapFile(info.GetFile(EncodePath(name)));
        }

        public IDirectoryInfo GetDirectory(params string[] name)
        {
            return WrapDirectory(info.GetDirectory(EncodePath(name)));
        }
    }
}
