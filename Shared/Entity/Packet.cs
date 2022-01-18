using Entity.ContentTypes;
using Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Entity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Packet
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Content;
        public ContentType ContentType;
        public Packet(byte[] content, ContentType contentType)
        {
            Content = new byte[1024 + 4];
            content.CopyTo(Content, 0);
            ContentType = contentType;
        }
    }
    public static class PacketFactory
    {
        private static IDictionary<Type, ContentType> _mappedTypes = new Dictionary<Type, ContentType>
        {
            {typeof(ObjectTransform), ContentType.ObjectTransform },
            {typeof(Message), ContentType.Message },
            {typeof(int), ContentType.Integer },
            {typeof(string), ContentType.String }
        };
        public static byte[] CreateBytes(object content)
        {
            var cType = ContentType.Undefined;
            _mappedTypes.TryGetValue(content.GetType(), out cType);
            Packet dgram = new Packet(StructUtility.StructToBytes(content), cType);
            return StructUtility.StructToBytes(dgram);
        }

        private static IDictionary<ContentType, Type> _reversedMappedTypes;
        static PacketFactory()
        {
            _reversedMappedTypes = new Dictionary<ContentType, Type>(_mappedTypes.Select(x => new KeyValuePair<ContentType, Type>(x.Value, x.Key)));
        }

        public static object CreateObject(byte[] bytes)
        {
            var datagram = StructUtility.BytesToStruct<Packet>(bytes);

            Type t;

            if (datagram.ContentType != ContentType.Undefined && _reversedMappedTypes.TryGetValue(datagram.ContentType, out t))
            {
                return StructUtility.BytesToStruct(datagram.Content, t);
            }

            throw new Exception("undefined content");
        }
    }
}
