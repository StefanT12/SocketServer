using Entity;
using SocketServer.Experiment.Factory;
using SocketServer.Experiment.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Server
{
    public class ServerMarshaller : IServerLayer
    {
        private readonly ISecureServerLayer _secureServerLayer;
        private IList<IObjectListener> _listeners = new List<IObjectListener>();
        public ServerMarshaller(Socket tcpSocket, Socket udpSocket, IServerLayer nextServerLayer, int maxConnections)
        {
            _secureServerLayer = new SecureServerLayer(tcpSocket, udpSocket,this , maxConnections);
        }
        public void AddListener(IObjectListener listener)
        {
            _listeners.Add(listener);
        }

        public Task<int> BroadcastToAsync(object content, string clientIpPort)
        {
            return _secureServerLayer.BroadcastToAsync(DatagramFactory.CreateBytes(content), clientIpPort);
        }

        public void HandleReceivedData(byte[] buff, string clientIpPort)
        {
            var obj = DatagramFactory.CreateObject(buff);
            foreach (var listener in _listeners)
            {
                listener.ReceiveObject(obj);
            }
        }

        public Task<int> SendToAsync(object content, string clientIpPort)
        {
            return _secureServerLayer.SendToAsync(DatagramFactory.CreateBytes(content), clientIpPort);
        }

        public void WhitelistClient(string ipPort, string symmetricKey)
        {
            _secureServerLayer.WhitelistClient(ipPort, symmetricKey);
        }
    }
}
