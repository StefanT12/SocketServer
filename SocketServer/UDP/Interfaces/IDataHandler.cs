using SocketServer.UDP.Entity.ContentTypes;

namespace SocketServer.UDP.Interfaces
{
    public interface IDataHandler
    {
        void TransformObject(ObjectTransform objectTransform);
    }
}
