using System.IO;

namespace SymbolSource.Processing.Basic
{
    public interface IPdbStoreConfig
    {
        string SrcSrvPath { get; set; }
    }

    public interface IPdbStoreManager : ISymbolStoreManager
    {
        PdbSrcSrvSection ReadSrcSrv(string pdbFilePath);
        void WriteSrcSrv(string pdbFilePath, PdbSrcSrvSection srcSrvSection);
    }

    public class PdbStoreManager : SymbolStoreManager, IPdbStoreManager
    {
        private readonly ProcessFactory processFactory = new ProcessFactory();
        private readonly IPdbStoreConfig pdbStoreConfig;

        public PdbStoreManager(IPdbStoreConfig pdbStoreConfig)
        {
            this.pdbStoreConfig = pdbStoreConfig;

            if (!File.Exists(PdbStrPath()))
                throw new IOException(string.Format("File not exists ('{0}')", PdbStrPath()));
        }

        public PdbSrcSrvSection ReadSrcSrv(string pdbFilePath) 
        {
            using (var pdbstrProcess = processFactory.Create(PdbStrPath(), "-r", string.Format(@"-p:""{0}""", pdbFilePath), "-s:srcsrv"))
            {
                pdbstrProcess.StartInfo.RedirectStandardOutput = true;
                pdbstrProcess.Start();

                string result = pdbstrProcess.StandardOutput.ReadToEnd();
                pdbstrProcess.WaitForExit();
                return PdbSrcSrvSection.Parse(result);
            }
        }

        public void WriteSrcSrv(string pdbFilePath, PdbSrcSrvSection srcSrvSection)
        {
            string tempFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempFile, srcSrvSection.ToString());

                using (var pdbstrProcess = processFactory.Create(PdbStrPath(), "-w", string.Format(@"-p:""{0}""", pdbFilePath), string.Format(@"-i:""{0}""", tempFile), "-s:srcsrv"))
                {
                    pdbstrProcess.Start();
                    pdbstrProcess.WaitForExit();
                }
            }
            finally
            {
                File.Delete(tempFile);
            }          
        }
        
        private string PdbStrPath()
        {
            return Path.Combine(pdbStoreConfig.SrcSrvPath, "pdbstr.exe");
        }       
    }
}