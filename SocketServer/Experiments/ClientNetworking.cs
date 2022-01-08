using SocketServer.UDP;
using SocketServer.UDP.Client;
using SocketServer.UDP.Entity;
using SocketServer.UDP.Interfaces;
using SocketServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer.Experiments
{
    public delegate void OnReceiveDataDelegate(Datagram data);
    public class SocketData
    {
        public int UdpOpenedPort { get; set; }
        public int TcpOpenedPort { get; set; }
        public string IpAddress { get; set; }
    }
    public class ClientNetworking : IDisposable
    {
        private const int BufSize = 32 * 2;
        private readonly OnReceiveDataDelegate _onReceiveCallback;
        private readonly ICrypto _crypto;
        public class StateObject
        {
            public Socket Socket { get; set; }
            public byte[] buffer = new byte[BufSize];
        }
        protected readonly StateObject StateObj = new StateObject();

        private readonly Socket UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp), TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public ClientNetworking(OnReceiveDataDelegate onReceiveCallback, ICrypto crypto)
        {
            _onReceiveCallback = onReceiveCallback;
            _crypto = crypto;
        }

        public SocketData SetNetworkingData()
        {
            UdpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            TcpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));

            return new SocketData
            {
                UdpOpenedPort = ((IPEndPoint)UdpSocket.LocalEndPoint).Port,
                TcpOpenedPort = ((IPEndPoint)TcpSocket.LocalEndPoint).Port,
                IpAddress = Dns.GetHostEntry(Dns.GetHostName())
                        .AddressList
                        .First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString()
        };
        }

        public async Task StartClientAsync(string serverAddress, int serverTcpPort, int serverUdpPort)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(serverAddress);

                await Task.WhenAll(TcpSocket.ConnectAsync(new IPEndPoint(ipAddress, serverTcpPort)), UdpSocket.ConnectAsync(new IPEndPoint(ipAddress, serverUdpPort)));

                Receive(UdpSocket);
                Receive(TcpSocket);
            }
            catch
            {
                Console.WriteLine("Could not connect to server...retrying in 10s");
                await Task.Delay(10000);
                await StartClientAsync(serverAddress, serverTcpPort, serverUdpPort);
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject()
                {
                    Socket = client
                };

                client.BeginReceive(state.buffer, 0, BufSize, 0, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.Socket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    _onReceiveCallback(StructUtility.BytesToStruct<Datagram>(_crypto.Decrypt(state.buffer)));
                }
                client.BeginReceive(state.buffer, 0, BufSize, 0, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void SendData<T>(T content) where T : struct
        {
            var data = _crypto.Encrypt(StructUtility.StructToBytes(
                new Datagram(
                    content: StructUtility.StructToBytes(content),
                    contentType: content.GetContentType())
                ));
            TcpSocket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, StateObj);
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                int bytesSent = TcpSocket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void Dispose()
        {
            TcpSocket.Close();
            UdpSocket.Close();
        }
    }
}
