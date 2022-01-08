using SocketServer.UDP.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using UDP.Base;

namespace SocketServer.UDP.Client
{
    public class Client : UdpSocket
    {
        private readonly IProcessor _processor;
        private IPEndPoint _remoteEndpoint;//sending
        public Client(IPAddress servAddress, int servPort, IProcessor processor) : base()
        {
            _remoteEndpoint = new IPEndPoint(servAddress, servPort);
            _processor = processor;
            Socket.Connect(servAddress, servPort);
        }
        private void SendAsyncCallback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            int bytes = Socket.EndSend(ar);
        }
        public void SendAsync<T>(T content) where T : struct
        {
            var processedData = _processor.Preprocess(content);
            Socket.BeginSend(processedData, 0, processedData.Length, SocketFlags.None, SendAsyncCallback, StateObj);
        }
        protected override void ReceiveAsyncCallback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            int bytes = Socket.EndReceiveFrom(ar, ref ReceivingEndpoint);
            Socket.BeginReceiveFrom(so.buffer, 0, BufSize, SocketFlags.None, ref ReceivingEndpoint, ReceiveAsyncCallback, so);
            _processor.Postprocess(so.buffer);
        }
    }
}
