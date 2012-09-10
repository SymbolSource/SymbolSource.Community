using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Ionic.Zip;
using log4net;
using SymbolSource.Server.Management.Client;
using PackageModel = SymbolSource;
using Version = SymbolSource.Server.Management.Client.Version;

namespace SymbolSource.Gateway.Core
{
    public abstract class GatewayManager : IGatewayManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GatewayManager));
        protected readonly IGatewayBackendFactory<IPackageBackend> backendFactory;
        protected readonly IGatewayConfigurationFactory configurationFactory;

        protected GatewayManager(IGatewayBackendFactory<IPackageBackend> backendFactory, IGatewayConfigurationFactory configurationFactory)
        {
            this.backendFactory = backendFactory;
            this.configurationFactory = configurationFactory;
        }

        public bool AuthenticateDownload(string company, string repository)
        {
            return company != "Basic" && (company != "Public" || repository.StartsWith("Private."));
        }

        public bool AuthenticateUpload(string company, string repository)
        {
            return company != "Basic";
        }

        public string Download(Caller caller, string company, string repository, string projectName, string versionName, string contentType)
        {
            var container = new Version { Company = company, Repository = repository, Project = projectName, Name = versionName };

            using (var session = backendFactory.Create(caller))
                return session.GetPackageLink(ref container, contentType);
        }

        public Version[] Index(Caller caller, string company, string repository)
        {
            var parent = new Repository { Company = company, Name = repository };

            using (var session = backendFactory.Create(caller))
            {
                return session.GetPackages(ref parent, GetPackageFormat());
            }
        }

        protected abstract string GetPackageFormat();

        public void Upload(Caller caller, Stream stream, string company, string repository)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("Uploading");

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
                    log.Error("Problem with saving package", exception);
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ClientException("Reading package failed", exception);
                }

                PackageProject package;
                IList<MetadataEntry> metadata;
                ILookup<ContentType, string> contents;

                try
                {
                    GetMetadata(path, repository, out package, out metadata, out contents);
                }
                catch (Exception exception)
                {
                    log.Error("Problem with getting metadata", exception);
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ClientException("Reading package metadata failed", exception);
                }

                try
                {
                    SynchronizePermissions(caller, company, package);
                }
                catch (Exception exception)
                {
                    log.Error("Problem with synchronizing permissions", exception);
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ClientException("Failed to synchronize permissions", exception);
                }

                try
                {
                    PrepareUpload(path, contents);
                }
                catch (Exception exception)
                {
                    log.Error("Problem with preparing package", exception);
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ClientException("Failed to modify package", exception);
                }

                try
                {
                    PerformUpload(caller, package, path);
                }
                catch (Exception exception)
                {
                    log.Error("Problem with uploading package", exception);
                    Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception, HttpContext.Current));
                    throw new ServerException("Package submission failed", exception);
                }
            }
            finally
            {
                Directory.Delete(path, true);
            }

            if (log.IsDebugEnabled)
                log.DebugFormat("Uploaded");
        }

        private void SynchronizePermissions(Caller caller, string company, PackageProject packageProject)
        {
            var permission = GetProjectPermission(caller, company, packageProject);

            if (permission.HasValue)
            {
                var configuration = configurationFactory.Create(company);

                if (string.IsNullOrEmpty(configuration.GatewayLogin) || string.IsNullOrEmpty(configuration.GatewayPassword))
                    throw new Exception("Missing gateway configuration");

                using (var session = backendFactory.Create(new Caller { Company = company, Name = configuration.GatewayLogin, KeyType = "API", KeyValue = configuration.GatewayPassword }))
                {
                    var project = new Project { Company = company, Repository = packageProject.Repository, Name = packageProject.Name };

                    if (permission.Value)
                    {
                        try
                        {
                            session.CreateProject(project);
                        }
                        catch (Exception)
                        {
                            //ignore if exists
                        }
                    }

                    try
                    {
                        session.SetProjectPermissions(new User { Company = caller.Company, Name = caller.Name }, project, new Permission { Grant = false, Read = false, Write = permission.Value });
                    }
                    catch (Exception)
                    {
                        //ignore if not exists
                    }

                    if (!permission.Value)
                        throw new Exception("External permission source denied publish access");
                }
            }
        }

        protected void SavePackage(Stream inputStream, string path)
        {
            using (var fileStream = new FileStream(GetFilePath(path), FileMode.CreateNew, FileAccess.Write))
                inputStream.CopyTo(fileStream);
        }

        protected abstract string GetFilePath(string path);

        protected abstract void GetMetadata(string path, string repository, out PackageProject project, out IList<MetadataEntry> metadata, out ILookup<ContentType, string> contents);

        protected abstract bool? GetProjectPermission(Caller caller, string companyName, PackageProject project);

        private string GetSymbolPackagePath(string path)
        {
            return GetFilePath(path) + "2";
        }

        private void PrepareUpload(string path, ILookup<ContentType, string> contents)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("Preparing package");

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

            if (log.IsDebugEnabled)
                log.DebugFormat("Prepared package");
        }

        private void RemoveFiles(ZipFile zip, IEnumerable<string> paths)
        {
            foreach (var path in paths)
                zip.RemoveEntry(path);
        }

        private void PerformUpload(Caller caller, PackageProject packageProject, string path)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("Uploading package");

            var packagePath = GetFilePath(path);
            var symbolPackagePath = GetSymbolPackagePath(path);

            var package = File.Exists(packagePath) ? File.ReadAllBytes(packagePath) : null;
            var symbolPackage = File.Exists(symbolPackagePath) ? File.ReadAllBytes(symbolPackagePath) : null;

            using (var session = backendFactory.Create(caller))
            {
                var report = session.UploadPackage(packageProject, GetPackageFormat(), package, symbolPackage);

                if (report.Summary != "OK")
                {
                    var builder = new StringBuilder();
                    builder.AppendLine(report.Summary);
                    builder.AppendLine();

                    if (!string.IsNullOrEmpty(report.Exception))
                    {
                        builder.AppendLine("Exception:");
                        builder.AppendLine(report.Exception);
                        builder.AppendLine();
                    }

                    if (!string.IsNullOrEmpty(report.Exception))
                    {
                        builder.AppendLine("Log:");
                        builder.AppendLine(report.Log);
                        builder.AppendLine();
                    }

                    throw new Exception(builder.ToString());
                }
            }

            if (log.IsDebugEnabled)
                log.DebugFormat("Uploaded upload package");
        }

        public void Hide(Caller caller, string company, string repository, string projectName, string versionName)
        {
            using (var session = backendFactory.Create(caller))
            {
                //var project = new Project { Company = company, Repository = repository, Name = projectName };
                //var versions = session.GetVersions(ref project);
                //var version = versions.SingleOrDefault(v => v.Name == versionName);

                //if (version == null)
                //    throw new ClientException("Specified version does not exist.", null);

                ////if (version.Hidden == true)
                ////    throw new ClientException("Version is already hidden.", null);

                //session.SetVersionHidden(ref version, true);
            }
        }

        public void Restore(Caller caller, string company, string repository, string projectName, string versionName)
        {
            using (var session = backendFactory.Create(caller))
            {
                //var project = new Project { Company = company, Repository = repository, Name = projectName };
                //var versions = session.GetVersions(ref project);
                //var version = versions.SingleOrDefault(v => v.Name == versionName);

                //if (version == null)
                //    throw new ClientException("Specified version does not exist.", null);

                ////if (version.Hidden == false)
                ////    throw new ClientException("Version is not hidden.", null);

                //session.SetVersionHidden(ref version, false);
            }
        }
    }
}
