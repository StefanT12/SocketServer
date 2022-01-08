using Entity;
using SocketServer.Cryptography.Entity;
using SocketServer.Server.Interfaces;
using SocketServer.UDP.Interfaces;
using SocketServer.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static SocketServer.Server.Interfaces.INetworkServer;

namespace SocketServer.Server
{
    public class ServerNetworking : INetworkServer
    {
        private static Dictionary<string, ClientMeta> _clients = new Dictionary<string, ClientMeta>();

        private const int _bufSize = 32 * 2;

        private readonly HandleReceivedData _handleReceivedData;
        private readonly ICrypto _crypto = new Cryptography.Cryptography();
        private readonly Socket UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp), TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private Task _acceptConnections;

        public ServerNetworking(HandleReceivedData handleReceivedData)
        {
            _handleReceivedData = handleReceivedData;
        }

        public ServerNetworkingData InitServer(int maxConnections)
        {
            var address = Dns.GetHostEntry(Dns.GetHostName())
                        .AddressList
                        .First(x => x.AddressFamily == AddressFamily.InterNetwork);

            TcpSocket.Bind(new IPEndPoint(address, 0));
            UdpSocket.Bind(new IPEndPoint(address, 0));
            TcpSocket.Listen(maxConnections);
            _acceptConnections = Task.Run(() => AcceptConnections());

            return new ServerNetworkingData
            {
                HostName = address.ToString(),
                TcpOpenedPort = ((IPEndPoint)TcpSocket.LocalEndPoint).Port,
                UdpOpenedPort = ((IPEndPoint)UdpSocket.LocalEndPoint).Port
            };
        }

        public async Task<bool> SendTcp<T>(T content, string ipPort) where T : struct
        {
            ClientMeta client;
            if (_clients.TryGetValue(ipPort, out client))
            {
                await client.TcpLock.WaitAsync();
                try
                {
                    await client.ConnectedSocket.SendAsync(_ReadyContentForClient(content, client), SocketFlags.None);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    client.TcpLock.Release();
                }
            }
            return false;
        }

        public async Task<bool> SendUdp<T>(T content, string ipPort) where T : struct
        {
            try
            {
                ClientMeta client; _clients.TryGetValue(ipPort, out client);

                await UdpSocket.SendToAsync(_ReadyContentForClient(content, client), SocketFlags.None, client.UdpEndpoint);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void WhitelistClient(int tcpPort, int udpPort, string ipAddress, string cryptoSymmetricKey)
        {
            _clients.TryAdd($"{ipAddress}:{tcpPort}", new ClientMeta
            {
                UdpEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), udpPort),
                CryptographicData = CryptographyUtility.GenerateData(cryptoSymmetricKey)
            });
        }

        #region receive
        private async Task AcceptConnections()
        {
            while (true)
            {
                var tcpClient = await TcpSocket.AcceptAsync();

                _ = Task.Run(() => DataReceiver(tcpClient));
            }
        }
        private async Task DataReceiver(Socket tcpClient)
        {
            var ipPort = tcpClient.RemoteEndPoint.ToString();

            if (!_clients.ContainsKey(ipPort))
            {
                return;//unauthorized asshole
            }
            _clients[ipPort].ConnectedSocket = tcpClient;

            var client = _clients[ipPort];

            byte[] buff = new byte[_bufSize];
            int readBytes = 0;

            while (true)
            {
                try
                {
                    if (!_IsClientSocketConnected(client.ConnectedSocket))
                    {
                        break;
                    }

                    readBytes = await client.ConnectedSocket.ReceiveAsync(buff, SocketFlags.None);

                    if (readBytes == 0)
                    {
                        continue;
                    }

                    _crypto.SetCryptingData(client.CryptographicData);

                    _ = Task.Run(() => _handleReceivedData(_crypto.Decrypt(buff)));
                }
                catch
                {
                    break;
                }
            }

            _clients.Remove(ipPort, out _);

        }
        #endregion

        private bool _IsClientSocketConnected(Socket client)
        {
            if (client.Connected)
            {
                if (client.Poll(0, SelectMode.SelectWrite) && !client.Poll(0, SelectMode.SelectError))
                {
                    byte[] buffer = new byte[1];
                    if (client.Receive(buffer, SocketFlags.Peek) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private byte[] _ReadyContentForClient<T>(T content, ClientMeta client) where T : struct
        {
            var datagram = StructUtility.StructToBytes(new Datagram(StructUtility.StructToBytes(content), content.GetContentType()));

            _crypto.SetCryptingData(client.CryptographicData);

            return _crypto.Encrypt(datagram);
        }
        public void Dispose()
        {
            UdpSocket.Close();
            TcpSocket.Close();
        }
        private class ClientMeta
        {
            public IPEndPoint UdpEndpoint { get; set; }
            public Socket ConnectedSocket { get; set; }
            public CryptographicData CryptographicData { get; set; }
            public SemaphoreSlim TcpLock = new SemaphoreSlim(0, 1);
        }
    }
}
