using SocketServer.Experiment.Factory;
using SocketServer.Experiment.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SocketServer.Utility;
namespace SocketServer.Experiment
{
    public class EcryptedClient : ISocketClient, ISocketListener
    {
        private IList<ISocketListener> _listeners = new List<ISocketListener>();

        private readonly ISocketClient _socketClient;
        private readonly ICryptoTransform _encryptor;
        private readonly ICryptoTransform _decryptor;
        public EcryptedClient(ISocketClient socketClient, ICryptoTransform encryptor, ICryptoTransform decryptor)
        {
            _socketClient = socketClient;
            _socketClient.AddListener(this);
            _encryptor = encryptor;
            _decryptor = decryptor;
        }

        public void AddListener(ISocketListener listener)
        {
            _listeners.Add(listener);
        }
       
        public Task<bool> SendBytes(ArraySegment<byte> bytes)
        {
            return _socketClient.SendBytes(_encryptor.Encrypt(bytes));
        }

        public Task StartClientAsync()
        {
            return _socketClient.StartClientAsync();
        }
        
        public void ReceivedBytes(byte[] bytes)
        {
            var decryptedBytes = _decryptor.Decrypt(bytes);
            foreach(var listener in _listeners)
            {
                listener.ReceivedBytes(decryptedBytes);
            }
        }
    }
}
