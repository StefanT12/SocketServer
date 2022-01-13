using SocketServer.Experiment.Server.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Server
{
    public class ServerUdpSocket : IServerConnectionlessSocketLayer
    {
        private readonly Socket _socket;
        public ServerUdpSocket(Socket socket)
        {
            _socket = socket;
        }
        public Task<int> SendBytes(ArraySegment<byte> bytes, IPEndPoint toEndpoint)
        {
            return _socket.SendToAsync(bytes, SocketFlags.None, toEndpoint);
        }
    }
}
