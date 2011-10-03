using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using SymbolSource.Processing.Basic.Projects;
using SymbolSource.Processing.Basic.Projects.FileInfos;
using Xunit;
using DirectoryInfo = SymbolSource.Processing.Basic.Projects.FileInfos.DirectoryInfo;

namespace SymbolSource.Processing.Basic.Tests
{
    public class SourceDiscoverTests
    {
        [Fact]
        public void Simple_File_Test_1()
        {
            RunTest(
                new SourceTuple(@"c:\kamil\kamil.cs", @"d:\kamil\kamil.cs")
                );
        }

        [Fact]
        public void Simple_File_Test_2()
        {
            RunTest(
                new SourceTuple(@"c:\kamil\test\kamil.cs", @"d:\kamil\test\kamil.cs"),
                new SourceTuple(@"c:\kamil\kamil.cs", @"d:\kamil\kamil.cs")
                );
        }

        [Fact]
        public void Simple_File_Test_3()
        {
            RunTest(
                new SourceTuple(@"c:\e\test\kamil.cs", @"g:\f\test\kamil.cs"),
                new SourceTuple(@"c:\e\kamil.cs", @"g:\f\kamil.cs")
                );
        }

        [Fact]
        public void Simple_File_Test_4()
        {
            RunTest(
                new SourceTuple(@"d:\projekty\kamil\Tools\Class1.cs", @"dummy\src\Tools\Class1.cs"),
                new SourceTuple(@"d:\projekty\kamil\_base\Tools\Class1.cs", @"dummy\src\_base\Tools\Class1.cs")
                );
        }

        private void RunTest(params SourceTuple[] files)
        {
            var srcSourcesFileInfos = files
                .Select(f => f.SrcSourcePath)
                .OrderBy(f => f);

            var pdbSourcesFiles = files
                .Select(f => f.PdbSourcePath)
                .OrderByDescending(f => f)
                .ToArray();

            var binaryStream = new MemoryStream();
            var symbolStream = new MemoryStream();

            var binaryPdbAddInfo = Mock.Of<IFileInfo>(p =>
                                                      p.Name == "Test.pdb"
                                                      && p.FullPath == @"dummy\lib\Test.pdb"
                                                      && p.GetStream(FileMode.Open) == symbolStream);
            var binaryDllAddInfo = Mock.Of<IFileInfo>(p =>
                                                      p.Name == "Test.dll"
                                                      && p.FullPath == @"dummy\lib\Test.dll"
                                                      && p.GetStream(FileMode.Open) == binaryStream
                                                      && p.ParentInfo.GetFile(binaryPdbAddInfo.Name) == binaryPdbAddInfo);

            var sourceExtractor = Mock.Of<ISourceExtractor>(p => p.ReadSources(binaryStream, symbolStream) == pdbSourcesFiles);
            var sourceStoreManager = Mock.Of<ISourceStoreManager>(s => s.ReadHash(null) == "__HASH__");
            var binaryStoreManager = Mock.Of<IBinaryStoreManager>(s => s.ReadBinaryHash(binaryStream) == "__BINARY_HASH__" && s.ReadPdbHash(binaryStream) == "__SYMBOL_HASH__");
            var symbolStoreManager = Mock.Of<ISymbolStoreManager>(s => s.ReadHash(symbolStream) == "__SYMBOL_HASH__");

            var sourceDiscover = new SourceDiscover(sourceExtractor, sourceStoreManager);
            var addInfoBuilder = new AddInfoBuilder(binaryStoreManager, symbolStoreManager, sourceDiscover);


            var directoryInfo = new TestDirectoryInfo(
                (new IFileInfo[] {binaryDllAddInfo, binaryPdbAddInfo})
                    .Concat(srcSourcesFileInfos.Select(p => Mock.Of<IFileInfo>(f => f.Name == Path.GetFileName(p) && f.FullPath == p)))
                );

            var allAddInfo = addInfoBuilder.Build(directoryInfo);

            foreach (var binaryInfo in allAddInfo.Binaries)
            {
                foreach (var sourceInfo in binaryInfo.SymbolInfo.SourceInfos)
                {
                    Assert.NotNull(sourceInfo.ActualPath);                    
                }
            }

            //foreach (var sourceTuple in files.Where(s => s.PdbSourcePath != null))
            //{
            //    var discovered = result.Single(d => d.OriginalPath == sourceTuple.PdbSourcePath);
            //    var actual = discovered.ActualPath != null ? discovered.ActualPath.FullPath : null;
            //    Assert.Equal(sourceTuple.SrcSourcePath, actual);
            //}
            
        }

        private class SourceTuple
        {
            public SourceTuple(string pdbSourcePath, string srcSourcePath)
            {
                PdbSourcePath = pdbSourcePath;
                SrcSourcePath = srcSourcePath;
            }

            public string PdbSourcePath { get; private set; }
            public string SrcSourcePath { get; private set; }
        }

        private class TestDirectoryInfo : Projects.FileInfos.DirectoryInfo
        {
            private readonly IEnumerable<IFileInfo> fileInfos;

            public TestDirectoryInfo(IEnumerable<IFileInfo> fileInfos) : base(null, null)
            {
                this.fileInfos = fileInfos;
            }

            public override string Name
            {
                get { return null; }
            }

            protected override IEnumerable<IDirectoryInfo> ExecuteGetDirectories()
            {
                return new IDirectoryInfo[0];
            }

            protected override IEnumerable<IFileInfo> ExecuteGetFiles()
            {
                return fileInfos;
            }
        }

    }
}