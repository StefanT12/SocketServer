//using Entity;
//using SocketServer.Cryptography;
//using SocketServer.Cryptography.Entity;
//using SocketServer.Experiment;
//using SocketServer.Experiments;
//using SocketServer.Server.Interfaces;
//using SocketServer.UDP.Interfaces;
//using SocketServer.Utility;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using System.Threading.Tasks;
//using static SocketServer.Server.Interfaces.INetworkServer;

//namespace SocketServer.Server
//{
//    public interface IServerSocket : IDisposable
//    {
//        int OpenedPort { get; }
//        /// <summary>
//        /// Sends data to a connectionless socket.
//        /// </summary>
//        /// <param name="content"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        Task<bool> SendAsync(ArraySegment<byte> content, IPEndPoint to);
//        /// <summary>
//        /// Sends data to a connected socket.
//        /// </summary>
//        /// <param name="content"></param>
//        /// <param name="toConnectedClient"></param>
//        /// <returns></returns>
//        Task<bool> SendAsync(ArraySegment<byte> content, Socket toConnectedClient);
//    }

//    public class ServerUdpSocket : IServerSocket
//    {
//        public ServerUdpSocket(string address) 
//        {

//        }

//        public int OpenedPort => throw new NotImplementedException();

//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<bool> SendAsync(ArraySegment<byte> content, IPEndPoint to)
//        {
//            return false;
//            //try
//            //{
//            //    int sentBytes = await Socket.SendToAsync(content, SocketFlags.None, to);

//            //    return sentBytes > 0;
//            //}
//            //catch
//            //{
//            //    return false;
//            //}
//        }

//        public Task<bool> SendAsync(ArraySegment<byte> content, Socket toConnectedClient)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class ServerTcpSocket :  IServerSocket
//    {
//        private const int _bufSize = 64;

//        private readonly Action<byte[], string> _handleReceivedData;
//        private readonly Action<string> _handleConnectionClosed;
//        private readonly Func<Socket, bool> _handleIncomingConnection;
//        private Socket Socket;
//        private Task _acceptConnections;
//        public ServerTcpSocket(string address, int maxConnections, Action<byte[], string> handleReceivedData, Action<string> _handleConnectionClosed, Func<Socket, bool> _handleIncomingConnection) : base(SocketClientType.Tcp, address)
//        {
//            _handleReceivedData = handleReceivedData;

//            Socket.Listen(maxConnections);

//            _acceptConnections = Task.Run(() => AcceptConnections());
//        }

//        private async Task AcceptConnections()
//        {
//            while (true)
//            {
//                var tcpClient = await Socket.AcceptAsync();

//                _ = Task.Run(() => DataReceiver(tcpClient));
//            }
//        }
//        private async Task DataReceiver(Socket clientSocket)
//        {
//            if (!_handleIncomingConnection(clientSocket))
//            {
//                return;//unauthorized asshole
//            }

//            byte[] buff = new byte[_bufSize];
//            int readBytes = 0;
//            var ipPort = clientSocket.RemoteEndPoint.ToString();

//            while (true)
//            {
//                try
//                {
//                    if (!_IsClientSocketConnected(clientSocket))
//                    {
//                        break;
//                    }
//                    readBytes = await clientSocket.ReceiveAsync(buff, SocketFlags.None);
//                    if (readBytes == 0)
//                    {
//                        continue;
//                    }
//                    _ = Task.Run(() => _handleReceivedData(buff, ipPort));
//                }
//                catch
//                {
//                    break;
//                }
//            }

//            _handleConnectionClosed(ipPort);
//        }
//        private bool _IsClientSocketConnected(Socket client)
//        {
//            if (client.Connected)
//            {
//                if (client.Poll(0, SelectMode.SelectWrite) && !client.Poll(0, SelectMode.SelectError))
//                {
//                    byte[] buffer = new byte[1];
//                    if (client.Receive(buffer, SocketFlags.Peek) == 0)
//                    {
//                        return false;
//                    }
//                    else
//                    {
//                        return true;
//                    }
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            else
//            {
//                return false;
//            }
//        }

//        public Task<bool> SendAsync(ArraySegment<byte> content, IPEndPoint to)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<bool> SendAsync(ArraySegment<byte> content, Socket toConnectedClient)
//        {
//            return await toConnectedClient.SendAsync(content, SocketFlags.None) > 0;
//        }
//    }

//    public enum ContentPriority
//    {
//        Low,
//        High
//    }

//    public class NetServerData
//    {
//        public int ServerUdpPort { get; set; }
//        public int ServerTcpPort { get; set; }
//        public string ServerAddress { get; set; }
//    }

//    public interface INetServer : IDisposable
//    {
//        void WhitelistClient(int tcpPort, int udpPort, string ipAddress, string cryptoSymmetricKey);
//        NetServerData GetServerData();
//        Task<bool> SendAsync<T>(T content, ContentPriority priority, string toIpAddressPort) where T : struct;
//    }
//    public class NetServer : INetServer
//    {
//        private static Dictionary<string, ClientMeta> _clients = new Dictionary<string, ClientMeta>();
//        private readonly string _address;
//        private readonly IServerSocket _udpSocket, _tcpSocket;
//        private readonly ICrypto _crypto;
//        private readonly Action<Datagram> _ReceivedData;
//        public NetServer(int maxConnections)
//        {
//            _address = Dns.GetHostEntry(Dns.GetHostName())
//                        .AddressList
//                        .First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();

//            _udpSocket = new ServerUdpSocket(_address);
//            _tcpSocket = new ServerTcpSocket(_address, 100, _HandleReceivedData, _HandleConnectionClosed, _HandleIncomingConnection);
//            _crypto = new Crypto();
//        }

//        public void WhitelistClient(int tcpPort, int udpPort, string ipAddress, string cryptoSymmetricKey)
//        {
//            _clients.TryAdd($"{ipAddress}:{tcpPort}", new ClientMeta
//            {
//                UdpEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), udpPort),
//                CryptographicData = CryptographyUtility.GenerateData(cryptoSymmetricKey)
//            });
//        }

//        public NetServerData GetServerData()
//        {
//            return new NetServerData
//            {
//                ServerAddress = _address,
//                ServerUdpPort = _udpSocket.OpenedPort,
//                ServerTcpPort = _tcpSocket.OpenedPort
//            };
//        }

//        public async Task<bool> SendAsync<T>(T content, ContentPriority priority, string toIpAddressPort) where T : struct
//        {
//            var client = _clients[toIpAddressPort];
//            _crypto.SetCryptingData(client.CryptographicData);

//            var datagram = new Datagram(StructUtility.StructToBytes(content), content.GetContentType());

//            var encryptedData = _crypto.Encrypt(StructUtility.StructToBytes(datagram));

//            if (priority == ContentPriority.Low)
//            {
//                return await _udpSocket.SendAsync(encryptedData, client.UdpEndpoint);
//            }
//            else
//            {
//                try
//                {
//                    await client.TcpLock.WaitAsync();
//                    return await _tcpSocket.SendAsync(encryptedData, client.ConnectedSocket);
//                }
//                catch
//                {
//                    return false;
//                }
//                finally
//                {
//                    client.TcpLock.Release();
//                }
//            }
//        }

//        private void _HandleReceivedData(byte[] data, string fromClientAddress)
//        {
//            _crypto.SetCryptingData(_clients[fromClientAddress].CryptographicData);
//            _ReceivedData(StructUtility.BytesToStruct<Datagram>(_crypto.Decrypt(data)));
//        }
//        private void _HandleConnectionClosed(string clientAddress)
//        {
//            _clients.Remove(clientAddress);
//        }

//        private bool _HandleIncomingConnection(Socket clientSocket)
//        {
//            var ipPort = clientSocket.RemoteEndPoint.ToString();

//            if (!_clients.ContainsKey(ipPort))
//            {
//                return false;//unauthorized asshole
//            }
//            _clients[ipPort].ConnectedSocket = clientSocket;

//            return true;
//        }

//        public void Dispose()
//        {
//            _tcpSocket.Dispose();
//            _udpSocket.Dispose();
//        }

//        private class ClientMeta
//        {
//            public IPEndPoint UdpEndpoint { get; set; }
//            public Socket ConnectedSocket { get; set; }
//            public CryptographicData CryptographicData { get; set; }
//            public SemaphoreSlim TcpLock = new SemaphoreSlim(1, 1);
//        }
//    }

//    public class ServerNetworking : INetworkServer
//    {
//        private static Dictionary<string, ClientMeta> _clients = new Dictionary<string, ClientMeta>();

//        private const int _bufSize = 32 * 2;

//        private readonly HandleReceivedData _handleReceivedData;
//        private readonly ICrypto _crypto = new Cryptography.Crypto();
//        private readonly Socket UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp), TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

//        private Task _acceptConnections;

//        public ServerNetworking(HandleReceivedData handleReceivedData)
//        {
//            _handleReceivedData = handleReceivedData;
//        }

//        public ServerNetworkingData InitServer(int maxConnections)
//        {
//            var address = Dns.GetHostEntry(Dns.GetHostName())
//                        .AddressList
//                        .First(x => x.AddressFamily == AddressFamily.InterNetwork);

//            TcpSocket.Bind(new IPEndPoint(address, 0));
//            UdpSocket.Bind(new IPEndPoint(address, 0));
//            TcpSocket.Listen(maxConnections);
//            _acceptConnections = Task.Run(() => AcceptConnections());

//            return new ServerNetworkingData
//            {
//                HostName = address.ToString(),
//                TcpOpenedPort = ((IPEndPoint)TcpSocket.LocalEndPoint).Port,
//                UdpOpenedPort = ((IPEndPoint)UdpSocket.LocalEndPoint).Port
//            };
//        }

//        public async Task<bool> SendTcp<T>(T content, string ipPort) where T : struct
//        {
//            ClientMeta client;
//            if (_clients.TryGetValue(ipPort, out client))
//            {
//                await client.TcpLock.WaitAsync();
//                try
//                {
//                    await client.ConnectedSocket.SendAsync(_ReadyContentForClient(content, client), SocketFlags.None);
//                    return true;
//                }
//                catch
//                {
//                    return false;
//                }
//                finally
//                {
//                    client.TcpLock.Release();
//                }
//            }
//            return false;
//        }

//        public async Task<bool> SendUdp<T>(T content, string ipPort) where T : struct
//        {
//            try
//            {
//                ClientMeta client; _clients.TryGetValue(ipPort, out client);

//                await UdpSocket.SendToAsync(_ReadyContentForClient(content, client), SocketFlags.None, client.UdpEndpoint);

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }
//        public void WhitelistClient(int tcpPort, int udpPort, string ipAddress, string cryptoSymmetricKey)
//        {
//            _clients.TryAdd($"{ipAddress}:{tcpPort}", new ClientMeta
//            {
//                UdpEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), udpPort),
//                CryptographicData = CryptographyUtility.GenerateData(cryptoSymmetricKey)
//            });
//        }

//        #region receive
//        private async Task AcceptConnections()
//        {
//            while (true)
//            {
//                var tcpClient = await TcpSocket.AcceptAsync();

//                _ = Task.Run(() => DataReceiver(tcpClient));
//            }
//        }
//        private async Task DataReceiver(Socket tcpClient)
//        {
//            var ipPort = tcpClient.RemoteEndPoint.ToString();

//            if (!_clients.ContainsKey(ipPort))
//            {
//                return;//unauthorized asshole
//            }
//            _clients[ipPort].ConnectedSocket = tcpClient;

//            var client = _clients[ipPort];

//            byte[] buff = new byte[_bufSize];
//            int readBytes = 0;

//            while (true)
//            {
//                try
//                {
//                    if (!_IsClientSocketConnected(client.ConnectedSocket))
//                    {
//                        break;
//                    }

//                    readBytes = await client.ConnectedSocket.ReceiveAsync(buff, SocketFlags.None);

//                    if (readBytes == 0)
//                    {
//                        continue;
//                    }

//                    _crypto.SetCryptingData(client.CryptographicData);

//                    _ = Task.Run(() => _handleReceivedData(_crypto.Decrypt(buff).ToArray()));
//                }
//                catch
//                {
//                    break;
//                }
//            }

//            _clients.Remove(ipPort, out _);

//        }
//        #endregion

//        private bool _IsClientSocketConnected(Socket client)
//        {
//            if (client.Connected)
//            {
//                if (client.Poll(0, SelectMode.SelectWrite) && !client.Poll(0, SelectMode.SelectError))
//                {
//                    byte[] buffer = new byte[1];
//                    if (client.Receive(buffer, SocketFlags.Peek) == 0)
//                    {
//                        return false;
//                    }
//                    else
//                    {
//                        return true;
//                    }
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            else
//            {
//                return false;
//            }
//        }
//        private ArraySegment<byte> _ReadyContentForClient<T>(T content, ClientMeta client) where T : struct
//        {
//            var datagram = StructUtility.StructToBytes(new Datagram(StructUtility.StructToBytes(content), content.GetContentType()));

//            _crypto.SetCryptingData(client.CryptographicData);

//            return _crypto.Encrypt(datagram);
//        }
//        public void Dispose()
//        {
//            UdpSocket.Close();
//            TcpSocket.Close();
//        }
//        private class ClientMeta
//        {
//            public IPEndPoint UdpEndpoint { get; set; }
//            public Socket ConnectedSocket { get; set; }
//            public CryptographicData CryptographicData { get; set; }
//            public SemaphoreSlim TcpLock = new SemaphoreSlim(1, 1);
//        }
//    }
//}
