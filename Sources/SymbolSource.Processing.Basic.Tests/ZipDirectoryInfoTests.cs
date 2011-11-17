using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SymbolSource.Processing.Basic.Projects;
using SymbolSource.Processing.Basic.Projects.FileInfos;
using Xunit;

namespace SymbolSource.Processing.Basic.Tests
{
    public class ZipDirectoryInfoTests : IDisposable
    {
        private readonly string path;
        private readonly string name;
        private readonly string directory;

        public ZipDirectoryInfoTests()
        {
            path = Path.ChangeExtension(Path.GetTempFileName(), "zip");
            name = Path.GetFileName(path);
            directory = Path.GetDirectoryName(path);

            using (var file = File.OpenWrite(path))
            using (var resource = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".test.zip"))
                resource.CopyTo(file);
        }

        public void Dispose()
        {
            File.Delete(path);
        }

        [Fact]
        public void Test()
        {
            using (var test = new ZipDirectoryInfo(new NullSpecialDirectoryHandler(), path))
            {
                AssertFile(test.GetFile("a.txt"), name + @"\a.txt", "a.txt", "a");
                AssertFile(test.GetFile("a", "b.txt"), name + @"\a\b.txt", "b.txt", "b");
                AssertFile(test.GetFile("a", "b", "c.txt"), name + @"\a\b\c.txt", "c.txt", "c");
            }
            //Assert.NotNull(zip.GetFile("metadata.xml").GetStream(FileMode.Open));
        }

        [Fact]
        public void TestInternal()
        {
            using (var test = new FileSystemDirectoryInfo(new InternalDirectoryInfoFactory(), directory))
            {
                var localPath = Path.Combine(Path.GetFileName(directory), name);
                AssertFile(test.GetFile(name, "a.txt"), localPath + @"\a.txt", "a.txt", "a");
                AssertFile(test.GetFile(name, "a", "b.txt"), localPath + @"\a\b.txt", "b.txt", "b");
                AssertFile(test.GetFile(name, "a", "b", "c.txt"), localPath + @"\a\b\c.txt", "c.txt", "c");
            }
            //Assert.NotNull(zip.GetFile("metadata.xml").GetStream(FileMode.Open));
        }

        [Fact]
        public void TestExternal()
        {
            using (var test = new FileSystemDirectoryInfo(new ExternalDirectoryInfoFactory(), directory))
            {
                var localPath = Path.Combine(Path.GetFileName(directory), name);
                AssertFile(test.GetFile(name, "a.txt"), localPath + @"\a.txt", "a.txt", "a");
                AssertFile(test.GetFile(name, "a", "b.txt"), localPath + @"\a\b.txt", "b.txt", "b");
                AssertFile(test.GetFile(name, "a", "b", "c.txt"), localPath + @"\a\b\c.txt", "c.txt", "c");
            }
            //Assert.NotNull(zip.GetFile("metadata.xml").GetStream(FileMode.Open));
        }

        private void AssertFile(IFileInfo file, string path, string name, string contents)
        {
            Assert.NotNull(file);
            Assert.Equal(name, file.Name);
            Assert.Equal(path, file.FullPath);
            
            using (var stream = file.GetStream(FileMode.Open))
            using (var reader = new StreamReader(stream))
                Assert.Equal(contents, reader.ReadToEnd());
   
        }
    }
}
