using System.IO;
using System.Linq;
using Moq;
using SymbolSource.Processing.Basic.Projects;
using Xunit;

namespace SymbolSource.Processing.Basic.Tests
{
    public class SourceDiscoverTests
    {
        public void Simple_File_Test()
        {
            RunTest(
                new SourceTuple(@"c:\kamil\kamil.cs", @"d:\kamil\kamil.cs")
                );
        }

        [Fact]
        public void Simple_File_Test_1()
        {
            RunTest(
                new SourceTuple(@"c:\kamil\test\kamil.cs", @"d:\kamil\test\kamil.cs"),
                new SourceTuple(@"c:\kamil\kamil.cs", @"d:\kamil\kamil.cs")
                );
        }

        [Fact]
        public void Simple_File_Test_2()
        {
            RunTest(
                new SourceTuple(@"c:\e\test\kamil.cs", @"g:\f\test\kamil.cs"),
                new SourceTuple(@"c:\e\kamil.cs", @"g:\f\kamil.cs")
                );
        }

        [Fact]
        public void Simple_File_Test_3()
        {
            RunTest(
                new SourceTuple(@"d:\projekty\kamil\Tools\Class1.cs", @"dummy\src\Tools\Class1.cs"),
                new SourceTuple(@"d:\projekty\kamil\_base\Tools\Class1.cs", @"dummy\src\_base\Tools\Class1.cs"),
                new SourceTuple(null, @"dummy\lib\kamil.dll")
                );
        }

        private void RunTest(params SourceTuple[] files)
        {
            var srcSourcesFileInfos = files
                .Where(f => f.SrcSourcePath != null)
                .Select(f => f.SrcSourcePath)
                .OrderBy(f => f)
                .Select(p => Mock.Of<IFileInfo>(f => f.FullPath == p))
                .ToArray();

            var pdbSourcesFiles = files
                .Where(f => f.PdbSourcePath != null)
                .Select(f => f.PdbSourcePath)
                .OrderByDescending(f => f)
                .ToArray();

            var binaryStream = new MemoryStream();
            var symbolStream = new MemoryStream();

            var binaryInfo = Mock.Of<IBinaryInfo>(b => b.File.GetStream(FileMode.Open) == binaryStream);
            var symbolInfo = Mock.Of<ISymbolInfo>(b => b.File.Name == "test.pdb" && b.File.GetStream(FileMode.Open) == symbolStream);

            var sourceExtractor = Mock.Of<ISourceExtractor>(p => p.ReadSources(binaryStream, symbolStream) == pdbSourcesFiles);
            var sourceStoreManager = Mock.Of<ISourceStoreManager>(s => s.ReadHash(null) == "__HASH__");

            var sourceDiscover = new SourceDiscover(sourceExtractor, sourceStoreManager);

            var result = sourceDiscover.FindSources(srcSourcesFileInfos, binaryInfo, symbolInfo)
                .ToArray();

            foreach (var sourceTuple in files.Where(s => s.PdbSourcePath != null))
            {
                var discovered = result.Single(d => d.OriginalPath == sourceTuple.PdbSourcePath);
                var actual = discovered.ActualPath != null ? discovered.ActualPath.FullPath : null;
                Assert.Equal(sourceTuple.SrcSourcePath, actual);
            }
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
    }
}