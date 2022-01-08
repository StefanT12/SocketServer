using SocketServer.UDP.Entity.ContentTypes;
using System;
using System.Collections.Generic;

namespace SocketServer.Utility
{
    public static class GetContentTypeUtility
    {
        private static Dictionary<Type, ContentType> _mappedTypes = new Dictionary<Type, ContentType>
        {
            {typeof(ObjectTransform), ContentType.ObjectTransform },
            {typeof(Message), ContentType.Message }
        };
        public static ContentType GetContentType<T>(this T t)
        {
            try
            {
                return _mappedTypes[typeof(T)];
            }
            catch
            {
                return ContentType.Undefined;
            }
        }
    }
}
