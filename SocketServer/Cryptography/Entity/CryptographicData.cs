using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Cryptography.Entity
{
    public struct CryptographicData
    {
        public ICryptoTransform Decryptor { get; set; }
        public ICryptoTransform Encryptor { get; set; }
    }
}
