using System.IO;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public abstract class Info : IInfo
    {
        private readonly DirectoryInfo parentInfo;

        protected Info(DirectoryInfo parentInfo)
        {
            this.parentInfo = parentInfo;
        }

        public IDirectoryInfo ParentInfo
        {
            get { return parentInfo; }
        }

        public string FullPath
        {
            get { return parentInfo != null ? Path.Combine(parentInfo.FullPath, Name) : Name; }
        }

        public abstract string Name { get; }

        public virtual void Dispose()
        {
        }
    }
}