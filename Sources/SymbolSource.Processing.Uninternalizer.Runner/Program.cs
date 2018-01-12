using System;
using System.IO;

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
