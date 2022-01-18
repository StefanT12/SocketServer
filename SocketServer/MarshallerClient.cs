using Client.Interfaces;
using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    public class MarshallerClient : IMarshaller, ISocketListener
    {
        private IList<IObjectListener> _listeners = new List<IObjectListener>();
        private readonly ISocket _socketClient;
        public MarshallerClient(ISocket socketClient)
        {
            _socketClient = socketClient;
            _socketClient.AddListener(this);
        }
        public void AddListener(IObjectListener listener)
        {
            _listeners.Add(listener);
        }

        public void ReceivedBytes(byte[] bytes)
        {
            var obj = PacketFactory.CreateObject(bytes);
            foreach (var listener in _listeners)
            {
                listener.ReceiveObject(obj);
            }
        }

        public Task<bool> SendObject(object obj)
        {
            return _socketClient.SendBytes(PacketFactory.CreateBytes(obj));
        }

        public Task StartClientAsync()
        {
            return _socketClient.StartAsync();
        }

    }
}
