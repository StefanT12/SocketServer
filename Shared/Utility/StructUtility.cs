using System;
using System.Runtime.InteropServices;

namespace Shared.Utility
{
    public static class StructUtility
    {
        public static byte[] ObjectToBytes(object obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] strctInBytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, strctInBytes, 0, size);
            Marshal.FreeHGlobal(ptr);

            return strctInBytes;
        }

        public static byte[] StructToBytes<T>(T strct)
        {
            int size = Marshal.SizeOf(strct);
            byte[] strctInBytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(strct, ptr, true);
            Marshal.Copy(ptr, strctInBytes, 0, size);
            Marshal.FreeHGlobal(ptr);

            return strctInBytes;
        }
        public static T BytesToStruct<T>(byte[] bytes)
        {
            return (T)BytesToStruct(bytes, typeof(T));
        }
        public static object BytesToStruct(byte[] bytes, Type t)
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure(handle.AddrOfPinnedObject(), t);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
