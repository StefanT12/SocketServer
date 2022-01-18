using Server.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class TcpServerSocket : ITcpServerSocket
    {
        private class TcpClient
        {
            public Socket ConnectedSocket;
            public SemaphoreSlim Lock = new SemaphoreSlim(1, 1);
        }

        private static ConcurrentDictionary<string, TcpClient> _clients = new ConcurrentDictionary<string, TcpClient>();

        private Task _acceptConnections = Task.CompletedTask;
        private IList<ITcpBytesListener> _listeners = new List<ITcpBytesListener>();
        private const int _bufSize = 2048;
        private readonly Socket _socket;
        private readonly IClientRegistry _clientRegistry;
        public TcpServerSocket(Socket socket, IClientRegistry clientRegistry, int maxConnections)
        {
            _clientRegistry = clientRegistry;
            _socket = socket;
            _socket.Listen(maxConnections);
        }
        public void Start()
        {

            if (_acceptConnections.IsCompleted)
            {
                _acceptConnections = Task.Run(() => AcceptConnections());
            }
            else
            {
                throw new Exception("Close TCP socket before starting it.");
            }
        }
        private async Task AcceptConnections()
        {
            while (true)
            {
                var clientSocket = await _socket.AcceptAsync();

                var key = clientSocket.RemoteEndPoint.ToString();

                if (_clientRegistry.ClientData.ContainsKey(key))
                {
                    _ = Task.Run(() => RunClientConnection(clientSocket, key));
                }
            }
        }

        private async Task RunClientConnection(Socket clientSocket, string clientIpPort)
        {
            byte[] buff = new byte[_bufSize];

            try
            {
                if(_clients.TryAdd(clientIpPort, new TcpClient{ ConnectedSocket = clientSocket }))
                {
                    foreach (var l in _listeners)
                    {
                        l.OnConnectionOpened(clientIpPort);
                    }

                    while (IsClientSocketConnected(clientSocket))
                    {
                        if (await clientSocket.ReceiveAsync(buff, SocketFlags.None) > 0)
                        {
                            foreach (var l in _listeners)
                            {
                                l.OnBytesReceived(buff, clientIpPort);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Server error: {e}");
                throw;
            }
            finally
            {
                if(_clients.TryRemove(clientIpPort, out _))
                {
                    foreach (var l in _listeners)
                    {
                        l.OnConnectionClosed(clientIpPort);
                    }
                }
            }
        }

        private bool IsClientSocketConnected(Socket client)
        {
            return client.Connected 
                && client.Poll(0, SelectMode.SelectWrite)
                && !client.Poll(0, SelectMode.SelectError)
                && client.Receive(new byte[1], SocketFlags.Peek) != 0;
        }

        public async Task<bool> SendBytes(ArraySegment<byte> bytes, string connectionId)
        {
            TcpClient m;
            if (_clients.TryGetValue(connectionId, out m))
            {
                await m.Lock.WaitAsync();
                try
                {
                    return await m.ConnectedSocket.SendAsync(bytes, SocketFlags.None) > 0;
                }
                finally
                {
                    m.Lock.Release();
                }
            }
            return false;
        }

        public void AddListener(ITcpBytesListener listener)
        {
            _listeners.Add(listener);
        }

        private class ClientMeta
        {
            public Socket ConnectedSocket { get; set; }

            public SemaphoreSlim TcpLock = new SemaphoreSlim(1, 1);
        }
    }
}
