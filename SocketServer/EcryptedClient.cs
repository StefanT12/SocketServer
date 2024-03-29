﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Client.Interfaces;
using Shared.Utility;

namespace Client
{
    public class EcryptedClient : ISocket, ISocketListener
    {
        private IList<ISocketListener> _listeners = new List<ISocketListener>();

        private readonly ISocket _socketClient;
        private readonly ICryptoTransform _encryptor;
        private readonly ICryptoTransform _decryptor;
        public EcryptedClient(ISocket socketClient, ICryptoTransform encryptor, ICryptoTransform decryptor)
        {
            _socketClient = socketClient;
            _socketClient.AddListener(this);
            _encryptor = encryptor;
            _decryptor = decryptor;
        }

        public void AddListener(ISocketListener listener)
        {
            _listeners.Add(listener);
        }

        public Task<bool> SendBytes(ArraySegment<byte> bytes)
        {
            return _socketClient.SendBytes(_encryptor.Encrypt(bytes));
        }

        public Task StartAsync()
        {
            return _socketClient.StartAsync();
        }

        public void ReceivedBytes(byte[] bytes)
        {
            var decryptedBytes = _decryptor.Decrypt(bytes);
            foreach (var listener in _listeners)
            {
                listener.ReceivedBytes(decryptedBytes);
            }
        }
    }
}
