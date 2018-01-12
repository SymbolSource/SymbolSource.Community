using System.Linq;
using Moq;
using SymbolSource.Processing.Basic;
using SymbolSource.Processing.Basic.Projects;
using Xunit;

namespace SymbolSource.Processing.Tools
{
    public class UrlTransformationTests
    {
        [Fact]
        public void TestFileFullPath()
        {
            var entry = Mocks.Of<IPackageEntry>()
                .Where(info => info.FullPath == @"a%60b\c%60d.t%60x%60t")
                .First();

            var wrapper = new TransformingWrapperPackageEntry(entry, new UrlTransformation());
            Assert.Equal(@"a`b\c`d.t`x`t", wrapper.FullPath);
        }

        [Fact]
        public void TestDirectoryGetFiles()
        {
            var file1 = Mocks.Of<IPackageEntry>()
               .Where(info => info.FullPath == @"a%60b\c%60d.txt")
               .First();

            var file2 = Mocks.Of<IPackageEntry>()
               .Where(info => info.FullPath == @"e%60f\g%60h.txt")
               .First();

            var directory = Mocks.Of<IPackageFile>()
                .Where(info => info.Entries == new[] { file1, file2 })
                .First();

            var wrapper = new TransformingWrapperPackageFile(directory, new UrlTransformation());
            var files = wrapper.Entries.ToArray();
            Assert.Equal(2, files.Length);
            Assert.Equal(@"a`b\c`d.txt", files[0].FullPath);
            Assert.Equal(@"e`f\g`h.txt", files[1].FullPath);
        }

        /*
        [Fact]
        public void TestDirectoryGetFileNotNull()
        {
            var calls = new List<string[]>();

            var file = Mocks.Of<IFileInfo>()
              .Where(info => info.FullPath == @"a%60b\c%60d.txt")
              .First();

            var directoryMock = new Mock<IDirectoryInfo>(MockBehavior.Strict);
            
            directoryMock
                .Setup(info => info.GetFile(It.IsAny<string[]>()))
                .Callback<string[]>(calls.Add)
                .Returns(file);

            var wrapper = new TransformingDirectoryWrapper(directoryMock.Object, new UrlTransformation());
            var actualFile = wrapper.GetFile("a`b", "c`d.txt");
            Assert.NotNull(actualFile);
            Assert.Equal(@"a`b\c`d.txt", actualFile.FullPath);
            Assert.Equal(1, calls.Count);
            Assert.Equal(2, calls[0].Length);
            Assert.Equal(@"a%60b", calls[0][0]);
            Assert.Equal(@"c%60d.txt", calls[0][1]);
        }

        [Fact]
        public void TestDirectoryGetFileNull()
        {
            var directoryMock = new Mock<IDirectoryInfo>(MockBehavior.Strict);

            directoryMock
                .Setup(info => info.GetFile(It.IsAny<string[]>()))
                .Returns(() => null);

            var wrapper = new TransformingDirectoryWrapper(directoryMock.Object, new UrlTransformation());
            var actualFile = wrapper.GetFile("a`b", "c`d.txt");
            Assert.Null(actualFile);
        }

        [Fact]
        public void TestDirectoryDirectories()
        {
            var directory1 = Mocks.Of<IDirectoryInfo>()
               .Where(info => info.FullPath == @"a%60b")
               .First();

            var directory2 = Mocks.Of<IDirectoryInfo>()
               .Where(info => info.FullPath == @"c%60d")
               .First();

            var directory = Mocks.Of<IDirectoryInfo>()
                .Where(info => info.GetDirectories() == new[] { directory1, directory2 })
                .First();

            var wrapper = new TransformingDirectoryWrapper(directory, new UrlTransformation());
            var directories = wrapper.GetDirectories().ToArray();
            Assert.Equal(2, directories.Length);
            Assert.Equal(@"a`b", directories[0].FullPath);
            Assert.Equal(@"c`d", directories[1].FullPath);
        }

        [Fact]
        public void TestDirectoryGetDirectoryNotNull()
        {
            var calls = new List<string[]>();

            var directory = Mocks.Of<IDirectoryInfo>()
              .Where(info => info.FullPath == @"a%60b\c%60d")
              .First();

            var directoryMock = new Mock<IDirectoryInfo>(MockBehavior.Strict);

            directoryMock
                .Setup(info => info.GetDirectory(It.IsAny<string[]>()))
                .Callback<string[]>(calls.Add)
                .Returns(directory);

            var wrapper = new TransformingDirectoryWrapper(directoryMock.Object, new UrlTransformation());
            var actualFile = wrapper.GetDirectory("a`b", "c`d");
            Assert.NotNull(actualFile);
            Assert.Equal(@"a`b\c`d", actualFile.FullPath);
            Assert.Equal(1, calls.Count);
            Assert.Equal(2, calls[0].Length);
            Assert.Equal(@"a%60b", calls[0][0]);
            Assert.Equal(@"c%60d", calls[0][1]);
        }

        [Fact]
        public void TestDirectoryGetDirectoryNull()
        {
            var directoryMock = new Mock<IDirectoryInfo>(MockBehavior.Strict);

            directoryMock
                .Setup(info => info.GetDirectory(It.IsAny<string[]>()))
                .Returns(() => null);

            var wrapper = new TransformingDirectoryWrapper(directoryMock.Object, new UrlTransformation());
            var actualFile = wrapper.GetDirectory("a`b", "c`d");
            Assert.Null(actualFile);
        }
        */ 
    }
}
