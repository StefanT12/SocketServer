using Server.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerHandler : IServerHandler, ITcpObjectListener
    {
        private class ListenerMap
        {
            public IList<object> Listeners = new List<object>();
            public MethodInfo Method { get; set; }
        }

        private readonly IDictionary<Type, ListenerMap> _listenerMaps = new ConcurrentDictionary<Type, ListenerMap>();

        private readonly IDictionary<string, ISenderHandler> _senderHandlers = new ConcurrentDictionary<string, ISenderHandler>();

        private readonly IServerMarshaller _serverMarshaller;
        public ServerHandler(IServerMarshaller serverMarshaller)
        {
            _serverMarshaller = serverMarshaller;
            _serverMarshaller.AddListener(this);
        }
        public void AddListener<T>(ITcpGenericListener<T> listener)
        {
            var genericType = typeof(T);
            if (!_listenerMaps.ContainsKey(genericType))
            {
                _listenerMaps.Add(genericType, new ListenerMap
                {
                    Method = typeof(ITcpGenericListener<T>).GetMethod(nameof(ITcpGenericListener<T>.ReceiveObject))
                });
            }
            _listenerMaps[genericType].Listeners.Add(listener);
        }

        public void OnConnectionClosed(string connectionId)
        {
            _senderHandlers.Remove(connectionId);
        }

        public void OnConnectionOpened(string connectionId)
        {
            _senderHandlers.Add(connectionId, new SenderHandler(connectionId, _serverMarshaller));
        }

        public void OnObjectReceived(object content, string connectionId)
        {
            var t = content.GetType();

            ListenerMap listenerMap;

            if (_listenerMaps.TryGetValue(t, out listenerMap))
            {
                var senderHandler = _senderHandlers[connectionId];
                foreach (var listener in listenerMap.Listeners)
                {
                    listenerMap.Method.Invoke(listener, new object[] { content, senderHandler });
                }
            }
        }

        private class SenderHandler : ISenderHandler
        {
            public string ConnectionId { get; private set; }

            private readonly IServerMarshaller _marshaller;

            public SenderHandler(string connectionId, IServerMarshaller marshaller)
            {
                ConnectionId = connectionId;
                _marshaller = marshaller;
            }

            public Task<bool> SendAsync(object content)
            {
                return _marshaller.SendObject(content, ConnectionId);
            }
        }
    }
}

