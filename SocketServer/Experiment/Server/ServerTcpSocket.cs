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
    public class ServerTcpSocket : IServerSocketLayer, IServerConnectionHandler
    {
        private const int _bufSize = 64;
        
        private readonly Socket _socket;

        private readonly IServerSocketLayer _nextSocketLayer;
        private readonly IServerConnectionHandler _nextConnectionHandler;
        public ServerTcpSocket(Socket socket, IServerSocketLayer nextSocketLayer, IServerConnectionHandler nextConnectionHandler , int maxConnections)
        {
            _socket = socket;
            _nextSocketLayer = nextSocketLayer;
            _nextConnectionHandler = nextConnectionHandler;
            _socket.Listen(maxConnections);
            Task.Run(() => AcceptConnections());
        }

        private async Task AcceptConnections()
        {
            while (true)
            {
                var tcpClient = await _socket.AcceptAsync();

                if (ValidateConnection(tcpClient))
                {
                    _ = Task.Run(() => RunClientConnection(tcpClient));
                }
            }
        }

        private async Task RunClientConnection(Socket clientSocket)
        {
            byte[] buff = new byte[_bufSize];
            var clientIpPort = clientSocket.RemoteEndPoint.ToString();

            try
            {
                while (IsClientSocketConnected(clientSocket))
                {
                    if (await clientSocket.ReceiveAsync(buff, SocketFlags.None) > 0)
                    {
                        await OnDataReceived(buff, clientIpPort);
                    }
                }
            }
            finally
            {
                await OnConnectionClosed(clientIpPort);
            }
        }

        private bool IsClientSocketConnected(Socket client)
        {
            return client.Connected &&
                client.Poll(0, SelectMode.SelectWrite ) &&
                !client.Poll(0, SelectMode.SelectError) &&
                client.Receive(new byte[1], SocketFlags.Peek) == 0;
        }

        public bool ValidateConnection(Socket clientSocket)
        {
            return _nextConnectionHandler.ValidateConnection(clientSocket);
        }

        public Task OnConnectionClosed(string clientIpPort)
        {
            return _nextConnectionHandler.OnConnectionClosed(clientIpPort);
        }

        public Task OnDataReceived(byte[] buff, string clientIpPort)
        {
            return _nextSocketLayer.OnDataReceived(buff, clientIpPort);
        }

        public Task<int> SendBytes(ArraySegment<byte> bytes, Socket toClient)
        {
            return toClient.SendAsync(bytes, SocketFlags.None);
        }
    }
}
