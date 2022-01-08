using SocketServer.Cryptography.Entity;
using SocketServer.UDP.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;

namespace SocketServer.Cryptography
{

    public class Cryptography : ICrypto
    {
        public CryptographicData CryptoData { get; set; }
        public void SetCryptingData(CryptographicData cryptingData)
        {
            CryptoData = cryptingData;
        }
        public byte[] Encrypt(byte[] plainBytes)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, CryptoData.Encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                }
                return ms.ToArray();
            }
        }
        public byte[] Decrypt(byte[] cypheredBytes)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, CryptoData.Decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(cypheredBytes, 0, cypheredBytes.Length);
                }
                return ms.ToArray();
            }
        }
        public void Dispose()
        {
            CryptoData.Encryptor.Dispose();
            CryptoData.Decryptor.Dispose();
        }
    }
}
