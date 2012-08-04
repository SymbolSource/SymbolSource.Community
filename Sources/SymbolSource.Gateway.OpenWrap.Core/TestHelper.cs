using System.IO;
using System.Net;
using OpenWrap;
using OpenWrap.Repositories;
using OpenWrap.Repositories.Http;
using OpenRasta.Client;

namespace SymbolSource.Gateway.OpenWrap.Core
{
    public class TestHelper
    {
        public void Push(string url, NetworkCredential credential, string name, Stream stream)
        {
            var repository = new IndexedHttpRepositoryFactory(new WebRequestHttpClient()).FromUserInput(url);
            using (var authenticated = repository.Feature<ISupportAuthentication>().WithCredentials(credential))
                repository.Feature<ISupportPublishing>().Publisher().Publish(name, stream);
        }
    }
}
