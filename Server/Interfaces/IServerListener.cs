using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface ITcpBytesListener
    {
        void OnBytesReceived(byte[] bytes, string connectionId);
        void OnConnectionClosed(string connectionId);
        void OnConnectionOpened(string connectionId);
    }

    public interface ITcpObjectListener
    {
        void OnObjectReceived(object content, string connectionId);
        void OnConnectionClosed(string connectionId);
        void OnConnectionOpened(string connectionId);
    }

    public interface ITcpGenericListener<T>
    {
        void OnConnectionClosed(ISenderHandler senderHandler);
        void OnConnectionOpened(ISenderHandler senderHandler);
        void ReceiveObject(T obj, ISenderHandler senderHandler);
    }

    public interface ISenderHandler
    {
        string ConnectionId { get; }
        Task<bool> SendAsync(object content);
    }
}
