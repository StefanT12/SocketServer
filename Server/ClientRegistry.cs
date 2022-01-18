using Server.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientRegistry : IClientRegistry
    {
        public ConcurrentDictionary<string, ClientMeta> ClientData { get; } = new ConcurrentDictionary<string, ClientMeta>();
    }
}
