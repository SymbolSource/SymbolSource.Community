using SymbolSource.Server.Management.Client;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {
        public void LogImageFileNotFound(string imageFileName, string symbolHash)
        {
            //throw new NotImplementedException();
        }

        public void LogSourceFileFound(SourceFile sourceFile, string computerName, string computerUser)
        {
            //throw new NotImplementedException();
        }

        public void LogImageFileFound(ImageFile imageFile)
        {
            //throw new NotImplementedException();
        }
    }
}