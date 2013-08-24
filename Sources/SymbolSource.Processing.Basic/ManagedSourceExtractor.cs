using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using Microsoft.Cci.Pdb;

namespace SymbolSource.Processing.Basic
{
    public class ManagedSourceExtractor : ISourceExtractor
    {
        public IList<string> ReadSources(string peFilePath, string pdbFilePath)
        {
            using (var stream = File.Open(pdbFilePath, FileMode.Open))
            {
                return ReadSources(null, stream);
            }
        }

        public IList<string> ReadSources(Stream peStream, Stream pdbStream)
        {
            var result = new List<string>();

            foreach (var obj1 in PdbFile.LoadFunctions(pdbStream, false))
                if (obj1.lines != null)
                    foreach (var obj2 in obj1.lines)
                        result.Add(obj2.file.name);

            return result.Where(IsValidSourceFileName).Distinct().ToArray();
        }

        private static bool IsValidSourceFileName(string sourceFileName)
        {
            return !string.IsNullOrEmpty(sourceFileName) && !IsTemporaryCompilerFile(sourceFileName);
        }

        private static bool IsTemporaryCompilerFile(string sourceFileName)
        {
            //the VB compiler will include temporary files in its pdb files.
            //the source file name will be similar to 17d14f5c-a337-4978-8281-53493378c1071.vb.
            return sourceFileName.EndsWith("17d14f5c-a337-4978-8281-53493378c1071.vb", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}