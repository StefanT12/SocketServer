using Entity;
using SocketServer.Utility;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer.NewStuff
{
    public static class SocketFactory
    {
        private static IPAddress _addressToBindTo = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
        private static Socket CreateSocketInternal(SocketClientType type)
        {
            Socket socket;
            if (type == SocketClientType.Tcp)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            return socket;
        }

        public static Socket CreateSocket(SocketClientType type)
        {
            Socket socket = CreateSocketInternal(type);

            socket.Bind(new IPEndPoint(_addressToBindTo, 0));

            return socket;
        }

        public static Socket CreateSocket(string address, SocketClientType type)
        {
            Socket socket = CreateSocketInternal(type);

            socket.Bind(new IPEndPoint(IPAddress.Parse(address), 0));

            return socket;
        }

        public static Socket CreateSocket(IPAddress address, SocketClientType type)
        {
            Socket socket = CreateSocketInternal(type);

            socket.Bind(new IPEndPoint(address, 0));

            return socket;
        }


        public enum SocketClientType
        {
            Udp,
            Tcp
        }
    }
}
