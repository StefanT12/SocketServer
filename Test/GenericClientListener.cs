using Entity.ContentTypes;
using SocketServer.Experiment.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class GenericClientListener : IGenericListener<int>, IGenericListener<string>, IGenericListener<Message>
    {
        public void ReceiveObject(string obj)
        {
            Console.WriteLine(obj);
        }

        public void ReceiveObject(int obj)
        {
            Console.WriteLine(obj);
        }

        public void ReceiveObject(Message obj)
        {
            Console.WriteLine(obj.GetType().ToString());
            Console.WriteLine(obj.Msg);
        }
    }
}
