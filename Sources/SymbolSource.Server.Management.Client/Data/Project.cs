namespace SymbolSource.Server.Management.Client
{
    public partial class Project
    {
        public Project()
        {
        }

        public Project(Repository repository, string name)
        {
            Company = repository.Company;
            Repository = repository.Name;
            Name = name;    
        }

        public Project(Version version)
        {
            Company = version.Company;
            Repository = version.Repository;
            Name = version.Project;
        }

        public override string ToString()
        {
             return string.Format("{0}/{1}/{2}", Company, Repository, Name);
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
