using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Nct.SignalR
{
    public class OnlineClientManager
    {

        public event EventHandler<OnlineClientEventArgs> ClientConnected;
        public event EventHandler<OnlineClientEventArgs> ClientDisconnected;

        private static object SyncObj = new object();

        private static OnlineClientManager _instance;

        protected ConcurrentDictionary<string, OnlineClient> Clients { get; }


        public OnlineClientManager()
        {
            Clients = new ConcurrentDictionary<string, OnlineClient>();
        }

        public static OnlineClientManager Instance
        {

            get
            {

                if (_instance == null)
                {
                    lock (SyncObj)
                    {
                        if (_instance == null)
                            _instance = new OnlineClientManager();
                    }

                }
                return _instance;
            }


        }



        public void Add(OnlineClient client)
        {

            Clients[client.ConnectionId] = client;
            ClientConnected?.Invoke(this, new OnlineClientEventArgs(client));

        }

        public bool Remove(string connectionId)
        {
            var isRemoved = Clients.TryRemove(connectionId, out OnlineClient client);
            if (isRemoved)
            {
                ClientDisconnected?.Invoke(this, new OnlineClientEventArgs(client));
            }
            return isRemoved;
        }

        public OnlineClient GetByConnectionIdOrNull(string connectionId)
        {

            Clients.TryGetValue(connectionId, out OnlineClient client);
            return client;

        }

        public List<OnlineClient> GetAllClients()
        {

            return Clients.Values.ToList();

        }


        public List<OnlineClient> GetAllByUserId(int? userId)
        {

            return GetAllClients().Where(c => userId == null || c.UserId == userId).ToList();
        }
    }
}
