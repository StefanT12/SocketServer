using SocketServer.Cryptography.Entity;
using SocketServer.Experiment.Server.Interfaces;
using SocketServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Server
{
    public class SecureServerLayer : ISecureServerLayer
    {
        private static Dictionary<string, ClientMeta> _clients = new Dictionary<string, ClientMeta>();

        private readonly IMultisocketsServerLayer _multisocketLayer;

        private readonly IServerLayer _nextServerLayer;
        public SecureServerLayer(Socket tcpSocket, Socket udpSocket, IServerLayer nextServerLayer, int maxConnections)
        {
            _multisocketLayer = new MultisocketsServerLayer(tcpSocket, udpSocket, this, maxConnections);
            _nextServerLayer = nextServerLayer;
        }
        public void WhitelistClient(string ipPort, string symmetricKey)
        {
            _clients.Add(ipPort, new ClientMeta
            {
                CryptographicData = CryptographyUtility.GenerateData(symmetricKey),
                UdpEndpoint = IPEndPoint.Parse(ipPort)
            });
        }
        public void OnConnectionClosed(string clientIpPort)
        {
            _clients.Remove(clientIpPort, out _);
        }

        public bool ValidateConnection(Socket clientSocket)
        {
            var ipPort = clientSocket.RemoteEndPoint.ToString();
            if (_clients.ContainsKey(ipPort))
            {
                _clients[ipPort].ConnectedSocket = clientSocket;
                return true;
            }
            return false;
        }

        public void OnDatReceived(byte[] buff, string clientIpPort)
        {
            //we always receive from whitelisted
            _nextServerLayer.HandleReceivedData(_clients[clientIpPort].CryptographicData.Decryptor.Decrypt(buff), clientIpPort);
        }

        public async Task<int> SendToAsync(ArraySegment<byte> bytes, string clientIpPort)
        {
            ClientMeta m;
            if (_clients.TryGetValue(clientIpPort, out m))
            {
                try
                {
                    await m.TcpLock.WaitAsync();
                    return await _multisocketLayer.SendBytes(m.CryptographicData.Encryptor.Encrypt(bytes), m.ConnectedSocket);
                }
                finally
                {
                    m.TcpLock.Release();
                }
            }
            return 0;
        }
        public Task<int> BroadcastToAsync(ArraySegment<byte> bytes, string clientIpPort)
        {
            ClientMeta m;
            if (_clients.TryGetValue(clientIpPort, out m))
            {
                return _multisocketLayer.SendBytes(m.CryptographicData.Encryptor.Encrypt(bytes), m.UdpEndpoint);
            }
            return Task.FromResult(0);
        }

        private class ClientMeta
        {
            public IPEndPoint UdpEndpoint { get; set; }
            public Socket ConnectedSocket { get; set; }
            public CryptographicData CryptographicData { get; set; }
            public SemaphoreSlim TcpLock = new SemaphoreSlim(1, 1);
        }
    }
}
