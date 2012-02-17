using System;
using System.Collections;
using System.Web;

namespace SymbolSource.Server.Management.Client
{
    public interface ILocalData
    {
        object this[object key] { get; set; }
        int Count { get; }
        void Clear();
    }

    public class Local
    {
        static readonly ILocalData data = new LocalData();

        public static ILocalData Data
        {
            get { return data; }
        }

        private class LocalData : ILocalData
        {
            [ThreadStatic]
            private static Hashtable localData;
            private static readonly object LocalDataHashtableKey = new object();

            private static Hashtable LocalHashtable
            {
                get
                {
                    if (!RunningInWeb)
                    {
                        if (localData == null)
                            localData = new Hashtable();
                        return localData;
                    }
                    else
                    {
                        var webHashtable = HttpContext.Current.Items[LocalDataHashtableKey] as Hashtable;
                        if (webHashtable == null)
                        {
                            webHashtable = new Hashtable();
                            HttpContext.Current.Items[LocalDataHashtableKey] = webHashtable;
                        }
                        return webHashtable;
                    }
                }
            }

            private static bool RunningInWeb
            {
                get { return HttpContext.Current != null; }
            }

            public object this[object key]
            {
                get { return LocalHashtable[key]; }
                set { LocalHashtable[key] = value; }
            }

            public int Count
            {
                get { return LocalHashtable.Count; }
            }

            public void Clear()
            {
                LocalHashtable.Clear();
            }
        }

    }
}