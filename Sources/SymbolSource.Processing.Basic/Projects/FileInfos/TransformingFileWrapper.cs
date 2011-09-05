using System.IO;

namespace SymbolSource.Processing.Basic.Projects.FileInfos
{
    public class TransformingFileWrapper : TransformingWrapper<IFileInfo>, IFileInfo
    {
        public TransformingFileWrapper(IFileInfo info, ITransformation transformation) 
            : base(info, transformation)
        {
        }

        public Stream GetStream(FileMode fileMode)
        {
            return transformation.DecodeContent(info.GetStream(fileMode));
        }

        public byte[] ReadAllBytes()
        {
            //TODO: po co ta metoda? sprawia tutaj problem
            return info.ReadAllBytes();
        }
    }
}
