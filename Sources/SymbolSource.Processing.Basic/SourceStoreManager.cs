using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SymbolSource.Processing.Basic
{
    public class SourceStoreManager : ISourceStoreManager
    {       
        public string ReadHash(Stream stream)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                {
                    byte[] retVal = md5.ComputeHash(stream);

                    var sb = new StringBuilder();
                    foreach (byte t in retVal)
                        sb.Append(t.ToString("x2"));
                    return sb.ToString();
                }
            }   
        }
    }
}