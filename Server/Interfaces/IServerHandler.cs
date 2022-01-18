﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IServerHandler
    {
        void AddListener<T>(ITcpGenericListener<T> listener);
    }
}
