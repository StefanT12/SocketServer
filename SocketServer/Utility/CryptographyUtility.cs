using SocketServer.Cryptography.Entity;
using System;
using System.IO;
using System.Security.Cryptography;

namespace SocketServer.Utility
{
    public static class CryptographyUtility
    {
        public static CryptographicData GenerateData(string key)
        {
            Aes algo = Aes.Create();
            algo.Padding = PaddingMode.Zeros;
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, new byte[] { 0x43, 0x87, 0x23, 0x72 });

            algo.Key = pdb.GetBytes(algo.KeySize / 8);
            algo.IV = pdb.GetBytes(algo.BlockSize / 8);

            var data = new CryptographicData
            {
                Encryptor = algo.CreateEncryptor(),
                Decryptor = algo.CreateDecryptor()
            };

            algo.Dispose();

            return data;
        }

        public static ArraySegment<byte> Encrypt(this ICryptoTransform cryptoTransform, ArraySegment<byte> plainBytes)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write, true))
                {
                    cs.Write(plainBytes.Array, 0, plainBytes.Count);

                }
                return new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }
        public static byte[] Decrypt(this ICryptoTransform cryptoTransform, byte[] cypheredBytes)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write, true))
                {
                    cs.Write(cypheredBytes, 0, cypheredBytes.Length);

                }
                return ms.GetBuffer();
            }
        }
    }
}
