using SocketServer.Experiment.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Interfaces
{
    public interface ISocket 
    {
        Task StartAsync();
        Task<bool> SendBytes(ArraySegment<byte> bytes);
        void AddListener(ISocketListener listener);
    }
}
