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
            Dictionary<uint, PdbTokenLine> outDictionary = null;
            foreach (var obj1 in PdbFile.LoadFunctions(pdbStream, out outDictionary))
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
            //these files seem to be different from non-temporary source files in that
            //the file name will not be rooted.

            //we want to exclude these files because other processing during import assumes
            //all source file names are absolute paths.  This seemed like the easiest place to fix the issue.
            return !Path.IsPathRooted(sourceFileName);
        }
    }
}