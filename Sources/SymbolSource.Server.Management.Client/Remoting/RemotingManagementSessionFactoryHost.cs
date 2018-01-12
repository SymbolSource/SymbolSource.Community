﻿using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SymbolSource.Server.Management.Client.Remoting
{
    public class RemotingManagementSessionFactoryHost : IDisposable
    {
        public static IManagementSessionFactory Activate(string uri)
        {
            return (IManagementSessionFactory)Activator.GetObject(typeof(IManagementSessionFactory), uri);
        }

        private readonly RemotingManagementSessionFactory factory;
        private readonly TcpServerChannel channel;

        public RemotingManagementSessionFactoryHost(string uri)
        {
            var parsedUri = new Uri(uri);

            if (parsedUri.Scheme != "tcp" || parsedUri.Host != "localhost" || parsedUri.Segments.Length != 2)
                throw new ArgumentException("uri");

            channel = new TcpServerChannel(parsedUri.Port);
            ChannelServices.RegisterChannel(channel, false);
            factory = new RemotingManagementSessionFactory();
            RemotingServices.Marshal(factory, parsedUri.Segments[1]);
        }

        public void Dispose()
        {
            channel.StopListening(null);
            RemotingServices.Disconnect(factory);
            ChannelServices.UnregisterChannel(channel);
        }

        public RemotingManagementSessionFactory Factory
        {
            get { return factory; }
        }
    }
}
