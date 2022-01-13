using SocketServer.Experiment.Server.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Server.Interfaces
{
    public interface IServerSocketLayer
    {
        Task OnDataReceived(byte[] buff, string clientIpPort);
        Task<int> SendBytes(ArraySegment<byte> bytes, Socket toClient);
    }
}
