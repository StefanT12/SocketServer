using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface ISocketListener
    {
        void ReceivedBytes(byte[] bytes);
    }
    public interface IObjectListener
    {
        void ReceiveObject(object obj);
    }
    public interface IGenericListener<T>
    {
        void ReceiveObject(T obj);
    }
}
