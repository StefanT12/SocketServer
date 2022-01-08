using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.UDP.Interfaces
{
    public interface IProcessor : IDisposable
    {
        void SetEncryption(ICryptoTransform encryptor, ICryptoTransform decryptor);
        byte[] Preprocess<T>(T content) where T : struct;
        void Postprocess(byte[] bytes);
    }
}
