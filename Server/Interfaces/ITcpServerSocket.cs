using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface ITcpServerSocket
    {
        Task<bool> SendBytes(ArraySegment<byte> bytes, string connectionId);
        void AddListener(ITcpBytesListener listener);
        void Start();
    }
}
