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

        private readonly ISourceExtractor sourceExtractor;
        private readonly ISourceStoreManager sourceStoreManager;

        public SourceDiscover(ISourceExtractor sourceExtractor, ISourceStoreManager sourceStoreManager)
        {
            this.sourceExtractor = sourceExtractor;
            this.sourceStoreManager = sourceStoreManager;
        }

        public IEnumerable<ISourceInfo> FindSources(IList<IFileInfo> fileInfos, IBinaryInfo peFile, ISymbolInfo pdbFile)
        {
            string pdbName = Path.GetFileNameWithoutExtension(pdbFile.File.Name);
            IList<string> originalPaths;
            using (var peStream = peFile.File.GetStream(FileMode.Open))
            using (var pdbStream = pdbFile.File.GetStream(FileMode.Open))
                originalPaths = sourceExtractor.ReadSources(peStream, pdbStream);

            if (originalPaths.Any())
            {
                string maxCommonOriginalPath = GetMaxCommonPath(originalPaths.Select(o => o.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)));

                if (originalPaths.Count == 1)
                    maxCommonOriginalPath = Path.GetDirectoryName(maxCommonOriginalPath);

                foreach (var originalPath in originalPaths)
                {
                    var actualPath = GetActualPaths(fileInfos, originalPath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar), maxCommonOriginalPath);

                    string keyPath = Path.Combine(pdbName, originalPath.Substring(maxCommonOriginalPath.Length+1)).Replace('\\', '/');

                    if(log.IsDebugEnabled)
                        log.DebugFormat("Found '{0}' -> '{1}' ('{2}') ", originalPath, actualPath!=null ? actualPath.FullPath : "NOT FOUND", keyPath);

                    if (actualPath != null)
                        using (var stream = actualPath.GetStream(FileMode.Open))
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

        private IFileInfo GetActualPaths(IList<IFileInfo> fileInfos, string originalPath, string maxCommonRealPath)
        {
            //Przypadek gdy tylko 1 zrodlo)
            if (maxCommonRealPath == originalPath)
                maxCommonRealPath = Path.GetDirectoryName(maxCommonRealPath);

            string tempLastPath = Path.GetFileName(originalPath);
            string tempFirstPath = Path.GetDirectoryName(originalPath);

            var candidates = GetCandidates(fileInfos, tempLastPath);

            while (candidates.Length > 1)
            {
                tempLastPath = Path.Combine(Path.GetFileName(tempFirstPath), tempLastPath.Trim(Path.DirectorySeparatorChar));
                tempFirstPath = Path.GetDirectoryName(tempFirstPath);

                candidates = GetCandidates(fileInfos, tempLastPath);
            }

            if (candidates.Any())
                return candidates.First();
            else
                return null;
            //throw new Exception(string.Format("Not found source for {0}", originalPath));
        }

        private IFileInfo[] GetCandidates(IList<IFileInfo> fileInfos, string fileName)
        {
            return fileInfos
                .Where(f => CheckCandidate(f, fileName))
                .ToArray();           
        }

        private bool CheckCandidate(IFileInfo fileInfo, string fileName)
        {
            string[] fileNameSplitted = fileName.Split(Path.DirectorySeparatorChar);
            string[] fullPathSplitted = fileInfo.FullPath.Split(Path.DirectorySeparatorChar);

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
