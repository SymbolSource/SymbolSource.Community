using System;
using System.IO;
using Xunit;

namespace SymbolSource.Processing.Basic.Tests
{
    public class SourceConverterTests
    {
        private static readonly byte[] asciiData = new byte[] { 0x61, 0x62, 0x63 };
        private static readonly byte[] utf8SuffixedData = new byte[] { 0xEF, 0xBB, 0xBF, 0x61, 0x62, 0xC2, 0xA9, 0xC2 };

        private Stream GetStream(string name)
        {
            var type = GetType();
            var fullName = string.Format("{0}.{1}.{2}.txt", type.Namespace, type.Name, name);
            return type.Assembly.GetManifestResourceStream(fullName);
        }

        private byte[] GetData(Stream stream)
        {
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return data;
        }

        private static void Compare(byte[] actual, params byte[] expected)
        {
            Assert.Equal(expected.Length, actual.Length);

            for (int i = 0; i < Math.Min(expected.Length, actual.Length); i++)
                Assert.Equal(expected[i], actual[i]);
        }

        private void TestFromFile(string name, byte[] originalExpected, byte[] convertedExpected)
        {
            using (var originalStream = GetStream(name))
            {
                var originalData = GetData(originalStream);
                originalStream.Position = 0;
                using (var convertedStream = SourceConverter.Convert(originalStream))
                {
                    var convertedData = GetData(convertedStream);
                    Compare(originalData, originalExpected);
                    Compare(convertedData, convertedExpected);
                }
            }
        }

        private void TestFromMemory(byte[] originalData, byte[] convertedExpected)
        {
            using (var originalStream = new MemoryStream(originalData))
            using (var convertedStream = SourceConverter.Convert(originalStream))
            {
                var convertedData = GetData(convertedStream);
                Compare(convertedData, convertedExpected);
            }
        }

        [Fact]
        public void TestAscii()
        {
            TestFromFile("Ascii", asciiData, asciiData);
        }

        [Fact]
        public void TestUtf8()
        {
            TestFromFile("Utf8", new byte[] { 0xEF, 0xBB, 0xBF, 0x61, 0x62, 0xC2, 0xA9 }, utf8SuffixedData);
        }

        [Fact]
        public void TestUtf8NotNeeded()
        {
            TestFromFile("Utf8NotNeeded", new byte[] { 0xEF, 0xBB, 0xBF, 0x61, 0x62, 0x63 }, asciiData);
        }

        //[Fact]
        //public void TestUtf8WithoutBom()
        //{
        //    TestFromFile("Utf8WithoutBom", new byte[] { 0x61, 0x62, 0xC2, 0xA9 }, utf8SuffixedData);
        //}

        [Fact]
        public void TestUtf16()
        {
            TestFromFile("Utf16", new byte[] { 0xFF, 0xFE, 0x61, 0x00, 0x62, 0x00, 0xA9, 0x00 }, utf8SuffixedData);
        }

        [Fact]
        public void TestUtf16BigEndian()
        {
            TestFromFile("Utf16BigEndian", new byte[] { 0xFE, 0xFF, 0x00, 0x61, 0x00, 0x62, 0x00, 0xA9 }, utf8SuffixedData);
        }

        [Fact]
        public void TestUtf16NotNeeded()
        {
            TestFromFile("Utf16NotNeeded", new byte[] { 0xFF, 0xFE, 0x61, 0x00, 0x62, 0x00, 0x63, 0x00 }, asciiData);
        }

        [Fact]
        public void TestWindows1252()
        {
            TestFromFile("Windows1252", new byte[] { 0x61, 0x62, 0xA9 }, new byte[] { 0x61, 0x62, 0xA9 });
        }

        [Fact]
        public void TestUtf8DoubleSuffx()
        {
            TestFromMemory(utf8SuffixedData, utf8SuffixedData);
        }
    }
}
