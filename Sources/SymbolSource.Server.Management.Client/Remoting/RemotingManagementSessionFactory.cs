using System;
using System.Collections.Generic;

namespace SymbolSource.Server.Management.Client.Remoting
{
    public class RemotingManagementSessionFactory : MarshalByRefObject, IManagementSessionFactory
    {
        private readonly IDictionary<Caller, Func<Caller, RemotingManagementSession>> sessionFactories;
        private readonly IEnumerable<Func<Caller, Caller>> partialGenerators;

        public RemotingManagementSessionFactory()
        {
            sessionFactories = new Dictionary<Caller, Func<Caller, RemotingManagementSession>>();

            partialGenerators =
                new Func<Caller, Caller>[]
                    {
                        caller => caller,
                        caller => new Caller {Company = caller.Company, KeyType = caller.KeyType, KeyValue = caller.KeyValue },
                        caller => new Caller {Company = caller.Company, Name = caller.Name, KeyType = caller.KeyType},
                        caller => new Caller {Company = caller.Company, Name = caller.Name},
                        caller => new Caller {Company = caller.Company},
                        caller => new Caller()
                    };
        }

        public Func<Caller, RemotingManagementSession> this[Caller caller]
        {
            get
            {
                return sessionFactories[caller];
            }
            set
            {
                if (value == null)
                    sessionFactories.Remove(caller);
                else
                    sessionFactories[caller] = value;
            }
        }

        private Caller Find(Caller caller)
        {
            foreach (var partialGenerator in partialGenerators)
            {
                var partialCaller = partialGenerator(caller);

                if (sessionFactories.ContainsKey(partialCaller))
                    return partialCaller;
            }

            return null;
        }

        public IManagementSession Create(Caller caller)
        {
            var partialCaller = Find(caller);

            if (partialCaller == null)
                throw new Exception(string.Format("No session registered for caller {0}", caller));

            return sessionFactories[partialCaller](caller);
        }

        public Caller GetUserByKey(string company, string type, string value)
        {
            //TODO: maybe GetUserByKey should be removed in favor if authenticating with Name == null
            return Find(new Caller { Company = company, KeyType = type, KeyValue = value });
        }

        public User Validate(Caller caller)
        {
            var partialCaller = Find(caller);

            if (partialCaller == null)
                return null;

            return new User();
        }

        public string DigestGenerateRequest(string realm)
        {
            var digest = new DigestHeader
                             {
                                 Realm = realm,
                                 Nonce = "test"
                             };

            return digest.ServerResponseHeader;
        }

        public Caller DigestValidateResponse(string company, string method, string response)
        {
            var digest = DigestHeader.Parse(response);

            var partialCaller = Find(new Caller
                                         {
                                             Company = company,
                                             Name = digest.Username,
                                             KeyType = "Password"
                                         });

            if (partialCaller == null)
                return null;

            if (partialCaller.KeyValue == null)
                return partialCaller;

            digest.Password = partialCaller.KeyValue;
            digest.Nonce = "test";

            if (digest.GetCalculatedResponse(method) != digest.Response)
                return null;

            return partialCaller;
        }
    }
}
