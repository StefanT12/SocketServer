using SocketServer.Experiment.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Experiment.Interfaces
{
    public interface IMarshaller
    {
        Task StartClientAsync();
        Task<bool> SendObject(object obj);
        void AddListener(IObjectListener listener);
    }
}
