namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public class TransformingWrapper<TInfo> where TInfo : IInfo
    {
        protected readonly TInfo info;
        protected readonly ITransformation transformation;

        public TransformingWrapper(TInfo info, ITransformation transformation)
        {
            this.info = info;
            this.transformation = transformation;
        }

        protected IFileInfo WrapFile(IFileInfo fileInfo)
        {
            if (fileInfo == null)
                return null;

            return new TransformingFileWrapper(fileInfo, transformation);
        }

        protected IDirectoryInfo WrapDirectory(IDirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
                return null;

            return new TransformingDirectoryWrapper(directoryInfo, transformation);
        }

        public IDirectoryInfo ParentInfo
        {
            get { return WrapDirectory(info.ParentInfo); }
        }

        public string FullPath
        {
            get { return transformation.DecodePath(info.FullPath); }
        }

        public string Name
        {
            get { return transformation.DecodePath(info.Name); }
        }

        public void Dispose()
        {
            info.Dispose();
        }
    }
}