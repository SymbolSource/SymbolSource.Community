using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Ionic.Zip;
using SymbolSource.Server.Management.Client;
using PackageModel = SymbolSource;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Gateway.Core
{
    public abstract class GatewayManager : IGatewayManager
    {
        protected readonly IGatewayBackendFactory<IPackageBackend> factory;

        protected GatewayManager(IGatewayBackendFactory<IPackageBackend> factory)
        {
            this.factory = factory;
        }

        public bool Authorize(string company, string repository)
        {
            return company != "Public" || repository.StartsWith("Private.");
        }

        public string Download(Caller caller, string company, string repository, string projectName, string versionName, string contentType)
        {
            var container = new Version { Company = company, Repository = repository, Project = projectName, Name = versionName };

            using (var session = factory.Create(caller))
                return session.GetPackageLink(ref container, contentType);
        }

        public Version[] Index(Caller caller, string company, string repository)
        {
            PrepareProject(caller, company, repository, null);

            var parent = new Repository { Company = company, Name = repository };

            using (var session = factory.Create(caller))
            {
                return session.GetPackages(ref parent, GetPackageFormat());
            }
        }

        protected abstract string GetPackageFormat();

        public void Upload(Caller caller, Stream stream, string company, string repository)
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);

            try
            {
                try
                {
                    SavePackage(stream, path);
                }
                catch (Exception exception)
                {
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ClientException("Reading package failed", exception);
                }

                PackageProject project;
                Version versionWithRecentMetadata;
                ILookup<ContentType, string> contents;

                try
                {
                    GetMetadata(path, repository, out project, out versionWithRecentMetadata, out contents);
                }
                catch (Exception exception)
                {
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ClientException("Reading package metadata failed", exception);
                }

                try
                {
                    PrepareProject(caller, company, repository, project.Name);
                }
                catch (Exception exception)
                {
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ClientException("Failed to obtain access to the package repository", exception);
                }

                Version version;
                try
                {
                    version = VerifyAccess(caller, company, repository, project.Name, project.Version.Name);
                    version.Metadata = versionWithRecentMetadata.Metadata;
                }
                catch (Exception exception)
                {
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ClientException("Failed to verify permissions for upload", exception);
                }

                try
                {
                    PrepareUpload(path, contents);
                }
                catch (Exception exception)
                {
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ClientException("Failed to modify package", exception);
                }

                try
                {
                    PerformUpload(caller,project, version, path);
                }
                catch (Exception exception)
                {
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ServerException("Package submission failed", exception);
                }
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        protected void SavePackage(Stream inputStream, string path)
        {
            using (var fileStream = new FileStream(GetFilePath(path), FileMode.CreateNew, FileAccess.Write))
                inputStream.CopyTo(fileStream);
        }

        protected abstract string GetFilePath(string path);

        protected abstract void GetMetadata(string path, string repository, out PackageProject project, out Version version,  out ILookup<ContentType, string> contents);

        private void PrepareProject(Caller caller, string companyName, string repositoryName, string projectName)
        {
            try
            {
                using (var session = factory.Create(caller))
                {
                    var repository = new Repository { Company = companyName, Name = repositoryName };
                    session.CreateOrUpdateRepository(repository);
                }
            }
            catch
            {
            }

            var configuration = new ConfigurationWrapper(companyName);
            //Wejdzie tylko gdy istnieje konfiguracja specjalnego użytkownika
            if (projectName != null && !string.IsNullOrEmpty(configuration.GatewayLogin) && !string.IsNullOrEmpty(configuration.GatewayPassword))
            {
                //Sprawdzenei czy jest specjalne zewnętrzne uprawnienie
                var syncPermission = GetProjectPermission(caller, companyName, repositoryName, projectName);

                //Jeżeli jest specjalne wewnętrzne uprawnienie
                if (syncPermission.HasValue)
                {
                    using (var session = factory.Create(companyName, configuration.GatewayLogin, "API", configuration.GatewayPassword))
                    {
                        var project = new Project { Company = companyName, Repository = repositoryName, Name = projectName };
                        var user = new User { Company = caller.Company, Name = caller.Name };
                        session.CreateOrUpdateProject(project);
                        session.SetProjectPermissions(user, project, new Permission { Read = false, Write = syncPermission.Value, Grant = false });
                    }
                }
            }
        }

        protected abstract bool? GetProjectPermission(Caller caller, string companyName, string repositoryName, string projectName);

        private Version VerifyAccess(Caller caller, string companyName, string repositoryName, string projectName, string versionName)
        {
            using (var session = factory.Create(caller))
            {
                try
                {
                    var project = new Project { Company = companyName, Repository = repositoryName, Name = projectName };
                    session.CreateProject(project);
                }
                catch (Exception)
                {
                    //ignore if exists
                }

                var version = new Version { Company = companyName, Repository = repositoryName, Project = projectName, Name = versionName };

                try
                {
                    //tu slowko ref wiec version bedzie zmodyfikowane - tu bedzie w wersji to co jest w bazie
                    var compilations = session.GetCompilationList(ref version);

                    if (compilations.Length > 0)
                        throw new Exception("Version not not available");
                }
                catch (Exception)
                {
                    session.CreateVersion(version);
                }

                return version;
            }
        }

        private string GetSymbolPackagePath(string path)
        {
            return GetFilePath(path) + "2";
        }

        private void PrepareUpload(string path, ILookup<ContentType, string> contents)
        {
            var packagePath = GetFilePath(path);
            var symbolPackagePath = GetSymbolPackagePath(path);

            if (contents[ContentType.Symbol].Any())
            {
                File.Copy(packagePath, symbolPackagePath);

                using (var zip = new ZipFile(symbolPackagePath))
                {
                    RemoveFiles(zip, contents[ContentType.Other]);
                    zip.Save();
                }
            }

            using (var zip = new ZipFile(packagePath))
            {
                RemoveFiles(zip, contents[ContentType.Symbol]);
                RemoveFiles(zip, contents[ContentType.Source]);
                zip.Save();
            }
        }

        private void RemoveFiles(ZipFile zip, IEnumerable<string> paths)
        {
            foreach (var path in paths)
                zip.RemoveEntry(path);
        }

        private void PerformUpload(Caller caller, PackageProject packageProject, Version version, string path)
        {
            PushPackage(caller, version, path, packageProject);

            var symbolPackagePath = GetSymbolPackagePath(path);

            if (File.Exists(symbolPackagePath))
                using (var session = factory.Create(caller))
                    session.CreateJob(File.ReadAllBytes(symbolPackagePath), packageProject);
        }

        protected abstract void PushPackage(Caller caller, Version version, string path, PackageProject metadata);

        public void Hide(Caller caller, string company, string repository, string projectName, string versionName)
        {
            using (var session = factory.Create(caller))
            {
                var project = new Project { Company = company, Repository = repository, Name = projectName };
                var versions = session.GetVersions(ref project);
                var version = versions.SingleOrDefault(v => v.Name == versionName);

                if (version == null)
                    throw new ClientException("Specified version does not exist.", null);

                //if (version.Hidden == true)
                //    throw new ClientException("Version is already hidden.", null);

                session.SetVersionHidden(ref version, true);
            }
        }

        public void Restore(Caller caller, string company, string repository, string projectName, string versionName)
        {
            using (var session = factory.Create(caller))
            {
                var project = new Project { Company = company, Repository = repository, Name = projectName };
                var versions = session.GetVersions(ref project);
                var version = versions.SingleOrDefault(v => v.Name == versionName);

                if (version == null)
                    throw new ClientException("Specified version does not exist.", null);

                //if (version.Hidden == false)
                //    throw new ClientException("Version is not hidden.", null);

                session.SetVersionHidden(ref version, false);
            }
        }
    }
}
