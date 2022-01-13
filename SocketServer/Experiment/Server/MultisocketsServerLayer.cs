using SocketServer.Experiment.Factory;
using SocketServer.Experiment.Interfaces;
using SocketServer.Experiment.Server.Interfaces;
using SocketServer.Experiment.Server.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Server
{
    public class MultisocketsServerLayer : IMultisocketsServerLayer, IServerConnectionHandler
    {
        private readonly ISecureServerLayer _nextSocketLayer;

        private readonly IServerSocketLayer _serverTcpSocket;
        private readonly IServerConnectionlessSocketLayer _serverUdpSocket;
        public MultisocketsServerLayer(Socket tcpSocket, Socket udpSocket, ISecureServerLayer nextSocketLayer, int maxConnections)
        {
            _serverTcpSocket = new ServerTcpSocket(tcpSocket, this, this, maxConnections);
            _serverUdpSocket = new ServerUdpSocket(udpSocket);
            _nextSocketLayer = nextSocketLayer;
        }

        public Task OnConnectionClosed(string clientIpPort)
        {
            return _nextSocketLayer.OnConnectionClosed(clientIpPort);
        }

        public Task<int> SendBytes(ArraySegment<byte> bytes, Socket toClient)
        {
            return _serverTcpSocket.SendBytes(bytes, toClient);
        }

        public Task<int> SendBytes(ArraySegment<byte> bytes, IPEndPoint toEndpoint)
        {
            return _serverUdpSocket.SendBytes(bytes, toEndpoint);
        }

        public bool ValidateConnection(Socket clientSocket)
        {
            return _nextSocketLayer.ValidateConnection(clientSocket);
        }

        Task IServerSocketLayer.OnDataReceived(byte[] buff, string clientIpPort)
        {
            return _nextSocketLayer.OnDataReceived(buff, clientIpPort);
        }
    }
}
