namespace SymbolSource.Server.Management.Client
{
    public partial class Reference
    {
        public string FullName
        {
            get
            {
                return string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", Name, Version, Culture, PublicKeyToken);
            }
        }
    }
}