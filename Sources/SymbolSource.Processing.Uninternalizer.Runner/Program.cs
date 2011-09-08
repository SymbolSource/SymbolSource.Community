using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymbolSource.Processing.Uninternalizer.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessDirectory("../../../../../Microsoft.Cci.Metadata/Sources");
        }

        private static void ProcessDirectory(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*.cs"))
            {
                Console.WriteLine(file);
                var uninternalizer = new Uninternalizer();
                uninternalizer.Uninternalize(file);
            }

            foreach (string directory in Directory.GetDirectories(path))
                ProcessDirectory(directory);
        }
    }
}
