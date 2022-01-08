using SocketServer.Cryptography.Entity;
using System;
using System.Security.Cryptography;

namespace SocketServer.UDP.Interfaces
{
    public interface ICrypto : IDisposable
    {
        void SetCryptingData(CryptographicData cryptingData);
        byte[] Decrypt(byte[] cypheredBytes);
        byte[] Encrypt(byte[] plainBytes);
    }
}
