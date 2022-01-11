//using Entity;
//using SocketServer.Cryptography;
//using SocketServer.Experiments;
//using SocketServer.UDP.Interfaces;
//using SocketServer.Utility;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;
//using static SocketServer.NewStuff.SocketFactory;

//namespace SocketServer.Experiment
//{
//    public interface INetClient : IDisposable
//    {
//        Task<bool> SendAsync<T>(T content) where T : struct;
//        Task StartClientAsync();
//        NetClientData GetClientData();
//    }

//    public class NetClientData
//    {
//        public int ClientUdpPort { get; set; }
//        public int ClientTcpPort { get; set; }
//        public string ClientAddress { get; set; }
//    }
//    public class NetClient : INetClient
//    {
//        private readonly ISocketClient _udpClient, _tcpClient;
//        private readonly ICrypto _crypto;

//        private readonly Action<Datagram> _clientOnReceiveCallback;
//        private readonly string _address;
//        private void _socketsReceiveCallback(byte[] data)
//        {
//            _clientOnReceiveCallback(StructUtility.BytesToStruct<Datagram>(_crypto.Decrypt(data)));
//        }
//        public NetClient(int udpServerPort, int tcpServerPort, string serverAddress, string symCryptoPass, Action<Datagram> clientOnReceiveCallback)
//        {
//            _address = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();

//            _udpClient = new SocketClient(_socketsReceiveCallback, SocketClientType.Udp, _address, serverAddress, udpServerPort);
//            _tcpClient = new SocketClient(_socketsReceiveCallback, SocketClientType.Tcp, _address, serverAddress, tcpServerPort);
//            _crypto = new Crypto()
//            {
//                CryptoData = CryptographyUtility.GenerateData(symCryptoPass)
//            };
//            _clientOnReceiveCallback = clientOnReceiveCallback;
//        }

//        public async Task StartClientAsync()
//        {
//            await Task.WhenAll(_udpClient.StartClientAsync(), _tcpClient.StartClientAsync());
//        }

//        public Task<bool> SendAsync<T>(T content) where T: struct
//        {
//            return _tcpClient.SendData(_crypto.Encrypt(DatagramFactory.CreateBytes(content)));
//        }

//        public NetClientData GetClientData()
//        {
//            return new NetClientData
//            {
//                ClientTcpPort = _tcpClient.OpenedPort,
//                ClientUdpPort = _udpClient.OpenedPort,
//                ClientAddress = _address
//            };
//        }

//        public void Dispose()
//        {
//            _tcpClient.Dispose();
//            _udpClient.Dispose();
//        }
//    }
//}
