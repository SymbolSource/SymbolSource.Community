using System.IO;
using System.Linq;
using Ionic.Zip;
using Moq;
using SymbolSource.Processing.Basic.Projects;
using Xunit;

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

            var sourceExtractor = Mock.Of<ISourceExtractor>(p => p.ReadSources(binaryStream, symbolStream) == pdbSourcesFiles);
            var sourceStoreManager = Mock.Of<ISourceStoreManager>(s => s.ReadHash(null) == "__HASH__");
            var binaryStoreManager = Mock.Of<IBinaryStoreManager>(s => s.ReadBinaryHash(binaryStream) == "__BINARY_HASH__" && s.ReadPdbHash(binaryStream) == "__SYMBOL_HASH__");
            var symbolStoreManager = Mock.Of<ISymbolStoreManager>(s => s.ReadHash(symbolStream) == "__SYMBOL_HASH__");

            var sourceDiscover = new SourceDiscover(sourceExtractor, sourceStoreManager);
            var addInfoBuilder = new AddInfoBuilder(binaryStoreManager, symbolStoreManager, sourceDiscover);

            var zipFile = new ZipFile();
            zipFile.AddEntry(@"dummy\lib\Test.pdb", symbolStream);
            zipFile.AddEntry(@"dummy\lib\Test.dll", binaryStream);

            foreach (var srcSourcesFileInfo in srcSourcesFileInfos)
                zipFile.AddEntry(srcSourcesFileInfo, srcSourcesFileInfo);

            var allAddInfo = addInfoBuilder.Build(new ZipPackageFile(zipFile));

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

    }
}