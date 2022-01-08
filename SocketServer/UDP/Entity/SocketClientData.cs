using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.UDP.Entity
{
    public struct SocketClientData
    {
        public IPAddress Address;
        public int Port;
        public ICryptoTransform Encryptor;
        public ICryptoTransform Decryptor;
    }
}
