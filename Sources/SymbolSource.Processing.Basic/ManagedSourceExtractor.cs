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
           
            return result.Where(c => !string.IsNullOrEmpty(c)).Distinct().ToArray();
        }
    }
}