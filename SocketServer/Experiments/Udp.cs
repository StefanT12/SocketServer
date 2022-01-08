//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using SocketServer.Utility;

//namespace SocketServer.UDP.Base
//{
//    abstract class Udp
//    {
//        public class StateObject
//        {
//            private const int _bufferSize = 256;
//            public byte[] buffer = new byte[_bufferSize];
//        }

//        private readonly StateObject _state = new StateObject();

//        protected readonly UdpClient UdpListener;
//        public readonly int OpenedPort;//port udp is configured to listen to
//        public readonly IPAddress Address;
//        protected IPEndPoint RemoteEndpoint;//sending
//        protected IPEndPoint EpFrom = new IPEndPoint(IPAddress.Any, 0);//receiving (from any) /// By default, servers should receive from anywhere

//        private AsyncCallback _recv = null;
//        public Udp()
//        {
//            UdpListener = new UdpClient(0, AddressFamily.InterNetwork);

//            Address = Dns.GetHostEntry(Dns.GetHostName())
//                      .AddressList
//                      .First(x => x.AddressFamily == AddressFamily.InterNetwork);
//            OpenedPort = ((IPEndPoint)UdpListener.Client.LocalEndPoint).Port;
//        }

//        protected void BeginReceivingAsync()
//        {
//            UdpListener.BeginReceive(_recv = (asyncRes) =>
//            {
//                var stateObj = (StateObject)asyncRes.AsyncState;
//                var bytes = UdpListener.EndReceive(asyncRes, ref EpFrom);
//                UdpListener.BeginReceive(_recv, stateObj);
//                //do something with these bytes
//                Console.WriteLine("RECV: {0}: {1}", EpFrom.ToString(), bytes.Length);
//            }, _state);
//        }
//    }

//    class Server : Udp
//    {
//        public Server() : base()
//        {
//            BeginReceivingAsync();
//        }

//        public void SendAsync(byte[] datagramInBytes, string addressTo, int portTo)
//        {
//            RemoteEndpoint.Address = IPAddress.Parse(addressTo);
//            RemoteEndpoint.Port = portTo;
//            Console.WriteLine("SENT: {0}: {1}", RemoteEndpoint.ToString(), datagramInBytes);
//            UdpListener.SendAsync(datagramInBytes, datagramInBytes.Length, RemoteEndpoint);
//        }
//    }

//    class Client : Udp
//    {
//        public Client(string serverAddress, int serverPort) : base()
//        {
//            RemoteEndpoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
//            EpFrom = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
//            BeginReceivingAsync();
//        }

//        public void SendAsync(byte[] datagramInBytes)
//        {
//            Console.WriteLine("SENT: {0}: {1}", RemoteEndpoint.ToString(), datagramInBytes.Length);
//            UdpListener.SendAsync(datagramInBytes, datagramInBytes.Length, RemoteEndpoint);
//        }
//    }
//}
