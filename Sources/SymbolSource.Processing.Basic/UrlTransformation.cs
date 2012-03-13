using System.IO;
using System.Web;
using SymbolSource.Processing.Basic.Projects;

namespace SymbolSource.Processing.Basic
{
    public class UrlTransformation : ITransformation
    {
        public string EncodePath(string path)
        {
            return HttpUtility.UrlEncode(path);
        }

        public string DecodePath(string path)
        {
            return HttpUtility.UrlDecode(path);
        }

        public Stream DecodeContent(Stream stream)
        {
            return stream;
        }

        public Stream EncodeContent(Stream stream)
        {
            return stream;
        }
    }
}
