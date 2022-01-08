using SocketServer.UDP.Entity;
using SocketServer.UDP.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UDP.Base;

namespace UDP
{
    public class Server: UdpSocket
    {
        protected readonly IProcessor _processor;
        protected IPEndPoint _remoteEndpoint;//sending
        private IDictionary<int, SocketClientData> _clientData = new Dictionary<int, SocketClientData>();
        public Server(IProcessor processor) : base() 
        {
            _processor = processor;
        }
        private void SendAsyncCallback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            int bytes = Socket.EndSend(ar);
        }
        public void SendAsync<T>(SocketClientData clientData, T content) where T : struct
        {
            _remoteEndpoint.Address = clientData.Address;
            _remoteEndpoint.Port = clientData.Port;
            _processor.SetEncryption(clientData.Encryptor, clientData.Decryptor);

            var processedData = _processor.Preprocess(content);

            Socket.BeginSendTo(processedData, 0, processedData.Length, SocketFlags.None, _remoteEndpoint, SendAsyncCallback, StateObj);
        }
        protected override void ReceiveAsyncCallback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            int bytes = Socket.EndReceiveFrom(ar, ref ReceivingEndpoint);
            Socket.BeginReceiveFrom(so.buffer, 0, BufSize, SocketFlags.None, ref ReceivingEndpoint, ReceiveAsyncCallback, so);

            var f = ReceivingEndpoint.ToString();

            _processor.Postprocess(so.buffer);
        }
    }
}
