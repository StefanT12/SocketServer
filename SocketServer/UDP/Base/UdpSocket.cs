using SocketServer.UDP;
using SocketServer.UDP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDP.Base
{
    public abstract class UdpSocket: IDisposable
    {
        protected Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        protected const int BufSize = 32 * 2;

        protected EndPoint ReceivingEndpoint = new IPEndPoint(IPAddress.Any, 0);

        protected readonly StateObject StateObj = new StateObject();

        public readonly int OpenedPort;//port udp is configured to listen to
        public readonly IPAddress Address;
        
        public class StateObject
        {
            public byte[] buffer = new byte[BufSize];
        }
        public UdpSocket()
        {
            Socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            Socket.BeginReceiveFrom(StateObj.buffer, 0, BufSize, SocketFlags.None, ref ReceivingEndpoint, ReceiveAsyncCallback, StateObj);

            Address = Dns.GetHostEntry(Dns.GetHostName())
                        .AddressList
                        .First(x => x.AddressFamily == AddressFamily.InterNetwork);
            OpenedPort = ((IPEndPoint)Socket.LocalEndPoint).Port;
        }
        protected abstract void ReceiveAsyncCallback(IAsyncResult ar);
        
        public virtual void Dispose()
        {
            Socket.Close();
        }
    }
}
