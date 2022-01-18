using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IDispatcher
    {
        void AddListener<T>(IGenericListener<T> listener);
    }
}
