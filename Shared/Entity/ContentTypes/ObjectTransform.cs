using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.ContentTypes
{
    public struct V3
    {
        public float x, y, z;
        public V3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public struct Qternion
    {
        public float x, y, z, w;
        public Qternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
    public struct ObjectTransform
    {
        public int Id;

        public V3 Position;

        public Qternion Rotation;

        public ObjectTransform(int Id, V3 Position, Qternion Rotation)
        {
            this.Id = Id;
            this.Position = Position;
            this.Rotation = Rotation;
        }
    }
}
