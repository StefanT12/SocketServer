using SocketServer.UDP.Entity.ContentTypes;
using SocketServer.Utility;
using System.Runtime.InteropServices;

namespace SocketServer.UDP.Entity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Datagram
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Content;
        public ContentType ContentType;
        public Datagram(byte[] content, ContentType contentType)
        {
            Content = new byte[32];
            content.CopyTo(Content, 0);
            ContentType = contentType;
        }
    }
    public static class DatagramFactory
    {
        public static byte[] CreateDatagramBytes<T>(T content) where T : struct
        {
            Datagram dgram = new Datagram(StructUtility.StructToBytes(content), content.GetContentType());
            return StructUtility.StructToBytes(dgram);
        }
    }
}
