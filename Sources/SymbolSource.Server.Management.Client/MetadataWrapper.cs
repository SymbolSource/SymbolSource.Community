using System.Collections.Generic;
using System.Linq;

namespace SymbolSource.Server.Management.Client
{
    public class MetadataWrapper
    {
        private readonly IList<MetadataEntry> list;

        public MetadataWrapper(IList<MetadataEntry> list)
        {
            this.list = list ?? new List<MetadataEntry>();
        }

        public string this[string key]
        {
            get
            {
                var entry = list.FirstOrDefault(e => e.Key.Equals(key));

                if (entry == null)
                    return null;

                return entry.Value;
            }
            set
            {
                var entry = list.FirstOrDefault(p => p.Key.Equals(key));

                if (entry == null)
                {
                    entry = new MetadataEntry();
                    entry.Key = key;
                    list.Add(entry);
                }

                entry.Value = value;

                if (string.IsNullOrEmpty(entry.Value))
                    list.Remove(entry);
            }
        }
    }
}
