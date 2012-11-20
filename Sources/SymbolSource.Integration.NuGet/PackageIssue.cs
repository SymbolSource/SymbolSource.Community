using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolSource.Integration.NuGet
{
    public enum PackageIssueLevel
    {
      Error,
      Warning,
    }

    public class PackageIssue
    {
        public PackageIssue(PackageIssueLevel level, string title, string description, string solution)
        {
            Solution = solution;
            Description = description;
            Title = title;
            Level = level;
        }

        public PackageIssueLevel Level { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Solution { get; private set; }
    }
}
