using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Server.Interfaces.Base
{
    public interface IServerConnectionHandler
    {
        bool ValidateConnection(Socket clientSocket);
        Task OnConnectionClosed(string clientIpPort);
    }
}
