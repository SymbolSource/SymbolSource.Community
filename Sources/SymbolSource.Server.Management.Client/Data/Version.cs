using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolSource.Server.Management.Client
{
    public partial class Version : IComparable<Version>
    {
        public Version()
        {
        }

        public Version(Project project, string name)
        {
            Company = project.Company;
            Repository = project.Repository;
            Project = project.Name;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}/{3}", Company, Repository, Project, Name);
        }

        public override bool Equals(object other)
        {
            return ToString().Equals(other.ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public int CompareTo(Version other)
        {
            int result =
                String.Compare(
                    string.Format("{0}/{1}/{2}", Company, Repository, Project),
                    string.Format("{0}/{1}/{2}", other.Company, other.Repository, other.Project),
                    StringComparison.OrdinalIgnoreCase);

            if (result != 0)
                return result;

            if (Name.Contains("-"))
            {
                if (other.Name.Contains("-"))
                {
                    return String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
                }
                return String.Compare(Name.Split('-')[0], other.Name, StringComparison.OrdinalIgnoreCase);
            }
            if (other.Name.Contains("-"))
                return String.Compare(Name, other.Name.Split('-')[0], StringComparison.OrdinalIgnoreCase);
            return String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
