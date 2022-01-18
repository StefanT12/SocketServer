using Entity;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerMarshaller : IServerMarshaller, ITcpBytesListener
    {
        private readonly IList<ITcpObjectListener> _listeners = new List<ITcpObjectListener>();
        private readonly ITcpServerSocket _socket;
        public ServerMarshaller(ITcpServerSocket socket)
        {
            _socket = socket;
            _socket.AddListener(this);
        }

        public void AddListener(ITcpObjectListener listener)
        {
            _listeners.Add(listener);
        }

        public void OnBytesReceived(byte[] bytes, string connectionId)
        {
            var obj = DatagramFactory.CreateObject(bytes);
            foreach (var l in _listeners)
            {
                l.OnObjectReceived(obj, connectionId);
            }
        }

        public void OnConnectionClosed(string connectionId)
        {
            foreach (var l in _listeners)
            {
                l.OnConnectionClosed(connectionId);
            }
        }

        public void OnConnectionOpened(string connectionId)
        {
            foreach (var l in _listeners)
            {
                l.OnConnectionOpened(connectionId);
            }
        }

        public Task<bool> SendObject(object content, string connectionId)
        {
            var bytes = DatagramFactory.CreateBytes(content);
            return _socket.SendBytes(bytes, connectionId);
        }

        public void Start()
        {
            _socket.Start();
        }
    }
}
