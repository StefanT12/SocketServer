using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IMarshaller
    {
        Task StartClientAsync();
        Task<bool> SendObject(object obj);
        void AddListener(IObjectListener listener);
    }
}
