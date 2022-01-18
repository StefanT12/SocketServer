using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IClientRegistry
    {
        ConcurrentDictionary<string, ClientMeta> ClientData { get; }
    }

    public class ClientMeta
    {
        public ICryptoTransform Decryptor { get; set; }
        public ICryptoTransform Encryptor { get; set; }
    }
}
