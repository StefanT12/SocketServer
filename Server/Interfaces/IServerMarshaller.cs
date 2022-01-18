using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IServerMarshaller
    {
        Task<bool> SendObject(object content, string connectionId);
        void AddListener(ITcpObjectListener listener);
        void Start();
    }
}
