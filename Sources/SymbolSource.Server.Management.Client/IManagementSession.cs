﻿using System;

namespace SymbolSource.Server.Management.Client.WebService
{
    public interface IManagementSession : IDisposable
    {
        Caller Caller { get; }
        //Company Company { get; }
        User User { get; }

        Company[] GetCompanies();
        void CreateCompany(Company company);
        void CreateCompany(User user, UserKey userKey, Plan plan);
        void UpdateCompany(Company company);
        void DeleteCompany(Company company);

        User[] GetUsers(ref Company company);
        void CreateUser(User user, UserKey userKey, Plan plan);
        void UpdateUser(User user);
        void DeleteUser(User user);
        void SendPasswordMail(string url);
        void ResetPassword(UserKey userKey);

        Repository[] GetRepositories(ref Company company);
        void CreateOrUpdateRepository(Repository repository);
        void CreateRepository(Repository repository);
        void UpdateRepository(Repository repository);
        void DeleteRepository(Repository repository);

        Project[] GetProjects(ref Repository repository);
        void CreateOrUpdateProject(Project project);
        void CreateProject(Project project);
        void UpdateProject(Project project);
        void DeleteProject(Project project);

        Version[] GetVersions(ref Project project);
        void CreateOrUpdateVersion(Version version);
        void CreateVersion(Version version);
        void UpdateVersion(Version version);
        void DeleteVersion(Version version);

        Version[] GetPackages(ref Repository repository, ref PackageFilter filter, string packageFormat);
        UploadReport UploadPackage(PackageProject package, string packageFormat, byte[] packageData, byte[] symbolPackageData);
        UploadReport[] GetUploadReports();
        string GetPackageLink(ref Version version, string contentType);

        CompanyPermission[] GetCompanyPermissions(Company company);
        RepositoryPermission[] GetRepositoryPermissions(Repository repository);
        ProjectPermission[] GetProjectPermissions(Project project);
        VersionPermission[] GetVersionPermissions(Version version);
        void SetCompanyPermissions(User targetUser, Company company, Permission permission);
        void SetRepositoryPermissions(User targetUser, Repository repository, Permission permission);
        void SetProjectPermissions(User targetUser, Project project, Permission permission);
        void SetVersionPermissions(User targetUser, Version version, Permission permission);

        //TODO: tak naprawdę to ignoruje autoryzację (gubiony caller) i umożliwia DoS
        Caller CreateUserByKey(string company, string type, string value);

        UserKey[] GetUserKeys(User targetUser);
        void AddUserKey(User targetUser, UserKey key);
        void RemoveUserKey(User targetUser, UserKey key);
                                                                                          
        Version[] GetVersionLastList();
        Compilation[] GetCompilationList(ref Version version);
        ImageFile[] GetImageFileList(ref Compilation compilation);
        ImageFile[] GetImageFileListByReference(ref Reference reference);
        ImageFile GetImageFile(string name, string symbolHash);
        SourceFile[] GetSourceFileList(ref ImageFile imageFile);
        Depedency[] GetDepedencyList(ref ImageFile imageFile);

        NodeImageFile[] GetNodeImageFiles(ref NodeImageFile nodeImageFile);

        string GetSourceFileLink(ref SourceFile sourceFile);
        string GetImageFileLink(ref ImageFile imageFile);
        string GetSymbolFileLink(ref ImageFile imageFile);

        string GetUserVisualStudioLink(User targetUser);
        Permissions GetUserPermissions(User targetUser);
       
        Version SetVersionHidden(ref Version version, bool hidden);

        Statistic[] GetStatistic(string[] names, StatisticPeriod period, DateTime? from, DateTime? to);
        string[] GetAvailableStatisticNames();

        void LogImageFileFound(ImageFile imageFile);
        void LogImageFileNotFound(string imageFileName, string symbolHash);
        void LogSourceFileFound(SourceFile sourceFile);

        Plan[] GetPlans(string type);
        void RemovePlan(string name, string type);
        void CreatePlan(string name, string type, decimal monthPrice, decimal yearPrice, int userLimit, int privateRepoLimit);
    }
}