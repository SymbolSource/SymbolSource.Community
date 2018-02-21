﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using NuGet;

namespace SymbolSource.Gateway.NuGet.Core
{
    public class TestHelper
    {
        public void Push(string url, string key, Stream stream)
        {
            var server = new PackageServer(url, "SymbolSource");

            var package = new ZipPackage(stream);
            var packageLength = package.GetStream().Length;

            server.PushPackage(key, package, packageLength, 5000, disableBuffering: false);
        }

        public int Count(string url, NetworkCredential credential)
        {
            var uri = new Uri(url);
            TestCredentialProvider.Instance.Credentials.Add(uri, credential);
            try
            {
                var repository = new PackageRepositoryFactory().CreateRepository(url);
                return repository.GetPackages().Count();
            }
            finally
            {
                TestCredentialProvider.Instance.Credentials.Remove(uri);
            }
        }
    }

    public class TestCredentialProvider : ICredentialProvider
    {
        static TestCredentialProvider()
        {
            HttpClient.DefaultCredentialProvider = Instance = new TestCredentialProvider(HttpClient.DefaultCredentialProvider);
        }

        public static TestCredentialProvider Instance { get; private set; }

        private TestCredentialProvider(ICredentialProvider provider)
        {
            this.provider = provider;
            Credentials = new Dictionary<Uri, ICredentials>();
        }
        private readonly ICredentialProvider provider;

        public IDictionary<Uri, ICredentials> Credentials { get; private set; }

        public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
        {
            if (Credentials.ContainsKey(uri))
                return Credentials[uri];

            return provider.GetCredentials(uri, proxy, credentialType, retrying);
        }
    }
}
