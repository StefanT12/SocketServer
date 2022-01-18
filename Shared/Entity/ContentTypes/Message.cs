using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Entity.ContentTypes
{
    public struct Message
    {
        public int Id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string Msg;
    }
}
