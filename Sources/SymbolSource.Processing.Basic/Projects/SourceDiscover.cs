using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace SymbolSource.Processing.Basic.Projects
{
    public class SourceDiscover
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SourceDiscover));

        private readonly ISourceExtractor pdbSourceExtractor;
        private readonly ISourceStoreManager sourceStoreManager;

        public SourceDiscover(ISourceExtractor pdbSourceExtractor, ISourceStoreManager sourceStoreManager)
        {
            this.pdbSourceExtractor = pdbSourceExtractor;
            this.sourceStoreManager = sourceStoreManager;
        }

        public IEnumerable<ISourceInfo> FindSources(IList<IPackageEntry> fileInfos, IBinaryInfo peFile, ISymbolInfo pdbFile)
        {
            string pdbName = Path.GetFileNameWithoutExtension(pdbFile.File.FullPath);
            IList<string> originalPaths;
            using (var peStream = peFile.File.Stream)
            using (var pdbStream = pdbFile.File.Stream)
                originalPaths = pdbSourceExtractor.ReadSources(peStream, pdbStream);

            if (originalPaths.Any())
            {
                string maxCommonOriginalPath = GetMaxCommonPath(originalPaths.Select(o => o.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)));
                if (originalPaths.Count == 1)
                    maxCommonOriginalPath = Path.GetDirectoryName(maxCommonOriginalPath);

                //Clean fileInfos
                var originalPathsExtensions = originalPaths.Select(Path.GetExtension).Distinct().ToArray();
                fileInfos = fileInfos.Where(f => originalPathsExtensions.Any(o => string.Equals(Path.GetExtension(f.FullPath), o, StringComparison.OrdinalIgnoreCase))).ToArray();


                string maxCommonFileInfosPath = GetMaxCommonPath(fileInfos.Select(o => o.FullPath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)));
                if (fileInfos.Count == 1)
                    maxCommonFileInfosPath = Path.GetDirectoryName(maxCommonFileInfosPath);


                foreach (var originalPath in originalPaths)
                {
                    var actualPath = GetActualPaths(fileInfos, originalPath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar), maxCommonOriginalPath, maxCommonFileInfosPath);

                    string keyPath = Path.Combine(pdbName, originalPath.Substring(maxCommonOriginalPath.Length+1)).Replace('\\', '/');

                    if(log.IsDebugEnabled)
                        log.DebugFormat("Found '{0}' -> '{1}' ('{2}') ", originalPath, actualPath!=null ? actualPath.FullPath : "NOT FOUND", keyPath);

                    if (actualPath != null)
                        using (var stream = actualPath.Stream)
                            yield return new SourceInfo(pdbFile)
                                             {
                                                 OriginalPath = originalPath,
                                                 ActualPath = actualPath,
                                                 KeyPath = keyPath,
                                                 Md5Hash = sourceStoreManager.ReadHash(stream)
                                             };
                    else
                        yield return new SourceInfo(pdbFile)
                                         {
                                             OriginalPath = originalPath,
                                             KeyPath = keyPath,
                                         };

                }
                yield break;
            }

            yield break;
        }

        private IPackageEntry GetActualPaths(IList<IPackageEntry> fileInfos, string originalPath, string maxCommonOriginalPath, string maxCommonFileInfosPath)
        {
            var candidates = GetActualPathStep1(fileInfos, originalPath, maxCommonOriginalPath, maxCommonFileInfosPath);
            
            if(candidates.Length==0)
                candidates = GetActualPathStep2(fileInfos, originalPath);

            if (candidates.Any())
                return candidates.First();
            else
                return null;
        }


        private IPackageEntry[] GetActualPathStep1(IList<IPackageEntry> fileInfos, string originalPath, string maxCommonOriginalPath, string maxCommonFileInfosPath)
        {
            var path = maxCommonFileInfosPath + originalPath.Substring(maxCommonOriginalPath.Length);
            var candidates = GetCandidates(fileInfos, path);
            if (candidates.Length == 1)
                return candidates;
            return new IPackageEntry[0];
        }

        private IPackageEntry[] GetActualPathStep2(IList<IPackageEntry> fileInfos, string originalPath)
        {
            string tempLastPath = Path.GetFileName(originalPath);
            string tempFirstPath = Path.GetDirectoryName(originalPath);

            var candidates = GetCandidates(fileInfos, tempLastPath);

            while (candidates.Length > 1)
            {
                tempLastPath = Path.Combine(Path.GetFileName(tempFirstPath), tempLastPath.Trim(Path.DirectorySeparatorChar));
                tempFirstPath = Path.GetDirectoryName(tempFirstPath);

                candidates = GetCandidates(fileInfos, tempLastPath);
            }
            return candidates;
        }

        private IPackageEntry[] GetCandidates(IList<IPackageEntry> fileInfos, string fileName)
        {
            return fileInfos
                .Where(f => CheckCandidate(f.FullPath, fileName))
                .ToArray();           
        }

        private bool CheckCandidate(string fullPath, string fileName)
        {
            string[] fileNameSplitted = fileName.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string[] fullPathSplitted = fullPath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

            //Przypadek gdy fullPath jest mniejszy od szukanego
            if (fileNameSplitted.Length > fullPathSplitted.Length)
                return false;


            for (int i = 0; i < fileNameSplitted.Length; i++ )
            {
                string lastFileName = fileNameSplitted[fileNameSplitted.Length - i - 1];
                string fullFileName = fullPathSplitted[fullPathSplitted.Length - i - 1];

                if (!lastFileName.Equals(fullFileName, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Szuka najdłuższej ścieżki która pasuje w każdym pliku
        /// </summary>
        private string GetMaxCommonPath(IEnumerable<string> originalPaths)
        {
            string maxCommonPath = originalPaths.First();
            foreach (string originalPath in originalPaths)
                maxCommonPath = GetMaxCommonPath(maxCommonPath, originalPath);
            maxCommonPath = maxCommonPath.Trim(Path.DirectorySeparatorChar);
            return maxCommonPath;
        }

        /// <summary>
        /// Pobiera najdłuższą wspólną ścieżke
        /// </summary>
        private string GetMaxCommonPath(string stringA, string stringB)
        {
            string[] a = stringA.Split(Path.DirectorySeparatorChar);
            string[] b = stringB.Split(Path.DirectorySeparatorChar);

            int diffIndex = 0;
            for (diffIndex = 0; diffIndex < Math.Min(a.Length, b.Length); diffIndex++)
            {
                if (!a[diffIndex].Equals(b[diffIndex], StringComparison.OrdinalIgnoreCase))
                    break;
            }
            if (diffIndex < a.Length && diffIndex < b.Length)
                a = a.Take(Math.Max(0, diffIndex)).ToArray();
            string[] toGenerate = a.Length < b.Length ? a : b;
            return string.Join(Path.DirectorySeparatorChar.ToString(), toGenerate);
        }
    }
}
