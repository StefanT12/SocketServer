using SocketServer.Experiment.Server.Interfaces.Base;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Server.Interfaces
{
    public interface ISecureServerLayer : IServerConnectionHandler
    {
        Task OnDataReceived(byte[] buff, string clientIpPort);
        void WhitelistClient(string ipPort, string symmetricKey);

        /// <summary>
        /// Sends a message to a client through a TCP protocol 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="clientIpPort"></param>
        /// <returns></returns>
        Task<int> SendToAsync(ArraySegment<byte> bytes, string clientIpPort);

        /// <summary>
        /// Broadcasts datagram to client UDP 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="clientIpPort"></param>
        /// <returns></returns>
        Task<int> BroadcastToAsync(ArraySegment<byte> bytes, string clientIpPort);
    }
}
