using System.IO;
using System.Text;

namespace SymbolSource.Processing.Basic
{
    public static class SourceConverter
    {
        public static Stream Convert(Stream originalStream)
        {
            var copyData = new byte[originalStream.Length];
            originalStream.Read(copyData, 0, copyData.Length);
            var copyStream = new MemoryStream(copyData);

            var encoding = DetectEncoding(copyData);

            if (encoding == null)
                return copyStream;

            var utf8Suffix = new byte[] { 0xC2 };

            if (encoding == Encoding.UTF8 && IsDataSuffixed(copyData, utf8Suffix))
                copyStream.SetLength(copyStream.Length - utf8Suffix.Length);

            using (var copyReader = new StreamReader(copyStream, encoding))
            {
                var originalText = copyReader.ReadToEnd();
                var asciiData = Encoding.ASCII.GetBytes(originalText);
                var asciiText = Encoding.ASCII.GetString(asciiData);

                if (originalText == asciiText)
                    return new MemoryStream(asciiData);

                return CreateSuffixedStream(originalText, Encoding.UTF8, utf8Suffix);
            }
        }

        private static Encoding DetectEncoding(byte[] data)
        {
            if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
                return Encoding.UTF8;

            if (data.Length >= 2 && data[0] == 0xFF && data[1] == 0xFE)
                return Encoding.Unicode;

            if (data.Length >= 2 && data[0] == 0xFE && data[1] == 0xFF)
                return Encoding.BigEndianUnicode;

            return null;
        }

        private static bool IsDataSuffixed(byte[] data, params byte[] suffix)
        {
            for (int i = 0; i < suffix.Length; i++)
                if (data[data.Length - 1 - i] != suffix[suffix.Length - 1 - i])
                    return false;

            return true;
        }

        private static MemoryStream CreateSuffixedStream(string originalText, Encoding encoding, params byte[] suffix)
        {
            var suffixedStream = new MemoryStream();
            var suffixedWriter = new StreamWriter(suffixedStream, encoding);

            suffixedWriter.Write(originalText);
            suffixedWriter.Flush();

            suffixedStream.Write(suffix, 0, suffix.Length);
            suffixedStream.Flush();

            suffixedStream.Position = 0;
            return suffixedStream;
        }
    }
}
