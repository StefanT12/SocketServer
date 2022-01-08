using System;
using System.Runtime.InteropServices;

namespace SocketServer.Utility
{
    public static class StructUtility
    {
        public static byte[] StructToBytes<T>(T strct) where T : struct
        {
            int size = Marshal.SizeOf(strct);
            byte[] strctInBytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(strct, ptr, true);
            Marshal.Copy(ptr, strctInBytes, 0, size);
            Marshal.FreeHGlobal(ptr);

            return strctInBytes;
        }
        public static T BytesToStruct<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
