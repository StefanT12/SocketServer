using SocketServer.UDP.Entity;
using SocketServer.UDP.Entity.ContentTypes;
using SocketServer.UDP.Interfaces;
using SocketServer.Utility;
using System.Security.Cryptography;

namespace SocketServer.UDP.Client
{
    public class Processor : IProcessor
    {
        private readonly ICrypto _crypto;
        private readonly IDataHandler _dataHandler;
        public Processor(ICrypto crypto, IDataHandler engineHandler)
        {
            _crypto = crypto;
            _dataHandler = engineHandler;
        }
        public void Postprocess(byte[] bytes)
        {
            var dgram = StructUtility.BytesToStruct<Datagram>(_crypto.Decrypt(bytes));
            switch (dgram.ContentType)
            {
                case ContentType.ObjectTransform:
                    _dataHandler.TransformObject(StructUtility.BytesToStruct<ObjectTransform>(dgram.Content));
                    break;
                default:
                    break;
            }
        }
        public byte[] Preprocess<T>(T content) where T : struct
        {
            Datagram dgram = new Datagram(StructUtility.StructToBytes(content), content.GetContentType());
            return _crypto.Encrypt(StructUtility.StructToBytes(dgram));
        }

        public void SetEncryption(ICryptoTransform encryptor, ICryptoTransform decryptor)
        {
            _crypto.SetCryptingData(new Cryptography.Entity.CryptographicData
            {
                Encryptor = encryptor,
                Decryptor = decryptor
            });
        }
        public void Dispose()
        {
            _crypto.Dispose();
        }
    }
}
