using System;
using System.IO;
using System.Linq;
using SymbolSource.Server.Management.Client;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Server.Basic
{
    public partial class BasicBackend
    {

        private string GetPathToImageFile(string name, string symbolHash)
        {
            string binaryIndexPath = Path.Combine(configuration.IndexPath, name);
            if(!Directory.Exists(binaryIndexPath))
                return null;

            string hashIndexPath = Path.Combine(binaryIndexPath, symbolHash + ".txt");
            if(!File.Exists(hashIndexPath))
                return null;

            var hashes = File.ReadAllLines(hashIndexPath)
                .Where(h => Directory.Exists(Path.Combine(configuration.DataPath, h)))
                .ToArray();
            
            File.WriteAllLines(hashIndexPath, hashes);

            return hashes.FirstOrDefault();
        }

        private string GetPathFromImageFile(ImageFile imageFile)
        {
            return Path.Combine(imageFile.Project, imageFile.Version, "Binaries", imageFile.Name, imageFile.SymbolHash);
        }

        private string GetPathFromSourceFile(SourceFile sourceFile)
        {
            return Path.Combine(sourceFile.Project, sourceFile.Version, "Sources", sourceFile.Path);
        }

        private ImageFile BuildImageFile(string path)
        {
            string[] parts = path.Split(Path.DirectorySeparatorChar);

            return new ImageFile
                       {
                           Repository = "Basic",
                           Project = parts[0],
                           Version = parts[1],
                           Platform = "Basic",
                           Mode = "Basic",
                           Name = parts[3],
                           SymbolHash = parts[4]
                       };
        }

        public ImageFile GetImageFile(string name, string symbolHash)
        {
            string pathImageFile = GetPathToImageFile(name, symbolHash);
            if(pathImageFile == null)
                return null;

            return BuildImageFile(pathImageFile);
        }

        public SourceFile[] GetSourceFileList(ref ImageFile imageFile)
        {
            var imageFileCopy = imageFile;

            string imagePath = GetPathFromImageFile(imageFile);
            string path = Path.Combine(configuration.DataPath, imagePath, imageFile.Name + ".txt");
            var sources = File.ReadAllLines(path)
                .Select(s => s.Split('|'))
                .Select(s => new SourceFile
                                 {
                                     Repository = imageFileCopy.Repository,
                                     Project = imageFileCopy.Project,
                                     Version = imageFileCopy.Version,
                                     Mode = imageFileCopy.Mode,
                                     Platform = imageFileCopy.Platform,
                                     ImageName = imageFileCopy.Name,
                                     Hash = "Basic",
                                     OriginalPath = s[0],
                                     Path = s[1],
                                 }
                )
                .ToArray();

            return sources;
        }

        public void CreateVersion(Version version)
        {
        }

        public Compilation[] GetCompilationList(ref Version version)
        {
            throw new NotImplementedException();
        }

        public void SetVersionHidden(ref Version version, bool hidden)
        {
            throw new NotImplementedException();
        }

        public Version[] GetVersions(ref Project project)
        {
            throw new NotImplementedException();
        }

        public void SetProjectPermissions(User targetUser, Project project, Permission permission)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdateProject(Project project)
        {
            throw new NotImplementedException();
        }

        public void CreateProject(Project project)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdateRepository(Repository repository)
        {
            throw new NotImplementedException();
        }

        public Version[] GetPackages(ref Repository repository, string packageFormat)
        {
            throw new NotImplementedException();
        }

        public Caller CreateUserByKey(string company, string type, string value)
        {
            throw new NotImplementedException();
        }
    }
}