using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using SymbolSource.Processing.Basic;

namespace SymbolSource.Integration.NuGet
{
    public interface ISymbolServerChecker
    {
        IDictionary<string, string> Check(string url);
    }

    public class SymbolServerChecker : ISymbolServerChecker
    {
        private readonly IPackage package;

        public SymbolServerChecker(IPackage package)
        {
            this.package = package;
        }

        public IDictionary<string, string> Check(string url)
        {
            var results = new Dictionary<string, string>();

            foreach (var file in package.GetFiles().Where(PackageHelper.IsBinaryFile))
                results[file.Path] = CheckFile(url, file);

            return results;
        }

        private static string CheckFile(string url, IPackageFile file)
        {
            var binary = new BinaryStoreManager();
            var symbol = new SymbolStoreManager();

            var name = Path.ChangeExtension(Path.GetFileName(file.Path), "pdb");
            var compressedName = name.Substring(0, name.Length - 1) + '_';

            string hash;

            using (var stream = file.GetStream())
                hash = binary.ReadPdbHash(stream);

            byte[] buffer;

            try
            {
                using (var client = new WebClient())
                    buffer = client.DownloadData(string.Format("{0}/{1}/{2}/{3}", url, name, hash, compressedName));
            }
            catch (WebException exception)
            {
                return ((HttpWebResponse)exception.Response).StatusCode.ToString();
            }

            //using (var stream = new MemoryStream(buffer))
            //    if (symbol.ReadHash(stream) != hash)
            //        return "MISMATCHED";

            return "FOUND";
        }
    }
}
