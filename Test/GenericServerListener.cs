using Entity.ContentTypes;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class GenericServerListener : ITcpGenericListener<Message>, ITcpGenericListener<int>
    {
        public void OnConnectionClosed(ISenderHandler senderHandler)
        {
            
        }

        public void OnConnectionOpened(ISenderHandler senderHandler)
        {

        }

        public void ReceiveObject(Message obj, ISenderHandler senderHandler)
        {
            
            Console.WriteLine($"Server received a message: {obj.Msg}, from connectionId: {senderHandler.ConnectionId}");
            senderHandler.SendAsync(new Message
            {
                Id = 123,
                Msg = "pula primita, ia-o inapoi!!!"
            });
        }

        public void ReceiveObject(int obj, ISenderHandler senderHandler)
        {
            throw new NotImplementedException();
        }
    }
}
