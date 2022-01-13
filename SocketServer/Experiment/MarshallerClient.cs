using Entity;
using SocketServer.Experiment.Factory;
using SocketServer.Experiment.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Experiment
{
    public class MarshallerClient : IMarshaller, ISocketListener
    {
        private IList<IObjectListener> _listeners = new List<IObjectListener>();
        private readonly ISocket _socketClient;
        public MarshallerClient(ISocket socketClient)
        {
            _socketClient = socketClient;
        }
        public void AddListener(IObjectListener listener)
        {
            _listeners.Add(listener);
        }

        public void ReceivedBytes(byte[] bytes)
        {
            var obj = DatagramFactory.CreateObject(bytes);
            foreach(var listener in _listeners)
            {
                listener.ReceiveObject(obj);
            }
        }

        public Task<bool> SendObject(object obj)
        {
            return _socketClient.SendBytes(DatagramFactory.CreateBytes(obj));
        }

        public Task StartClientAsync()
        {
            return _socketClient.StartAsync();
        }

    }
}
