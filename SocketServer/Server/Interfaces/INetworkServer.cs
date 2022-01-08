using System;
using System.Threading.Tasks;

namespace SocketServer.Server.Interfaces
{
    public interface INetworkServer : IDisposable
    {
        public delegate Task HandleReceivedData(byte[] data);
        public class ServerNetworkingData
        {
            public string HostName { get; set; }
            public int TcpOpenedPort { get; set; }
            public int UdpOpenedPort { get; set; }
        }
        ServerNetworkingData InitServer(int maxConnections);
        void WhitelistClient(int tcpPort, int udpPort, string ipAddress, string cryptoSymmetricKey);
        Task<bool> SendTcp<T>(T content, string ipPort) where T : struct;
        Task<bool> SendUdp<T>(T content, string ipPort) where T : struct;
    }
}
