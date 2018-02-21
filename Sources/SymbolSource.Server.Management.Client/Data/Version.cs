using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolSource.Server.Management.Client.WebService
{
    public partial class Version
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
    }
}
