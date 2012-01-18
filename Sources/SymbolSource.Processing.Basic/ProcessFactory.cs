using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SymbolSource.Processing.Basic
{
    public class ProcessFactory
    {
        public Process Create(string programName, params string[] arguments)
        {
            var result = new Process();
            result.StartInfo.FileName = GetProgramName(programName);
            result.StartInfo.Arguments = GetProgramArguments(programName, arguments);
            result.StartInfo.UseShellExecute = false;
            return result;
        }

        private string GetProgramName(string programName)
        {
            if (IsWindows)
                return programName;
            else
                return "wine";
        }

        private string GetProgramArguments(string programName, params string[] arguments)
        {
            if (IsWindows)
                return string.Join(" ", arguments);
            else
            {
                var list = new List<string> {string.Format(@"""{0}""", programName)};
                list.AddRange(arguments);
                return string.Join(" ", list.ToArray());                    
            }
                
        }

        private bool IsWindows
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return !((p == 4) || (p == 6) || (p == 128));
            }
        }

    }
}