using Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public class EncryptionServerLayer : ITcpBytesListener, ITcpServerSocket
    {
        private readonly ITcpServerSocket _socket;
        private readonly IList<ITcpBytesListener> _listeners = new List<ITcpBytesListener>();
        private readonly IClientRegistry _clientRegistry;
        public EncryptionServerLayer(ITcpServerSocket socket, IClientRegistry clientRegistry)
        {
            _socket = socket;
            _socket.AddListener(this);
            _clientRegistry = clientRegistry;
        }
        public void AddListener(ITcpBytesListener listener)
        {
            _listeners.Add(listener);
        }

        public void OnBytesReceived(byte[] bytes, string connectionId)
        {
            var decrypted =  _clientRegistry.ClientData[connectionId].Decryptor.Decrypt(bytes);
            foreach (var l in _listeners)
            {
                l.OnBytesReceived(decrypted, connectionId);
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

        public Task<bool> SendBytes(ArraySegment<byte> bytes, string connectionId)
        {
            return _socket.SendBytes(_clientRegistry.ClientData[connectionId].Encryptor.Encrypt(bytes), connectionId);
        }

        public void Start()
        {
            _socket.Start();
        }
    }
}
