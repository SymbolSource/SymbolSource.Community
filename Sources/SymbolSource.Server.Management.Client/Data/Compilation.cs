namespace SymbolSource.Server.Management.Client.WebService
{
    public partial class Compilation
    {
        public Compilation()
        {
        }

        public Compilation(Version version, string profile, string mode, string platform)
        {
            Company = version.Company;
            Repository = version.Repository;
            Project = version.Project;
            Version = version.Name;
            Profile = profile;
            Mode = mode;
            Platform = platform;
        }

        public override bool Equals(object obj)
        {
            var compilation = obj as Compilation;

            if (compilation == null)
                return false;

            return compilation.Project.Equals(Project)
                   && compilation.Version.Equals(Version)
                   && compilation.Mode.Equals(Mode)
                   && compilation.Platform.Equals(Platform);
        }

        public override int GetHashCode()
        {
            return Project.GetHashCode()
                   *Version.GetHashCode()
                   *Mode.GetHashCode()
                   *Platform.GetHashCode();
        }
    }
}
