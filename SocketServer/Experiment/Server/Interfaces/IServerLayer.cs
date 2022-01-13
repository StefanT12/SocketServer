using SocketServer.Experiment.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Server.Interfaces
{
    public interface IServerLayer
    {
        void AddListener(IObjectListener listener);
        void HandleReceivedData(byte[] buff, string clientIpPort);
        void WhitelistClient(string ipPort, string symmetricKey);

        /// <summary>
        /// Sends a message to a client through a TCP protocol 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="clientIpPort"></param>
        /// <returns></returns>
        Task<int> SendToAsync(object content, string clientIpPort);

        /// <summary>
        /// Broadcasts datagram to client UDP 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="clientIpPort"></param>
        /// <returns></returns>
        Task<int> BroadcastToAsync(object content, string clientIpPort);
    }
}
