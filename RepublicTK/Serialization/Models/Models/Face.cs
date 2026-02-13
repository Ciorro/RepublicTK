using RepublicTK.Serialization.Common.Models;
using System.Numerics;

namespace RepublicTK.Serialization.Models.Models
{
    public struct Face
    {
        internal const int SIZE = 40;

        public Vector4 Plane { get; set; }
        public BoundingBox Bounds { get; set; }

        public Face(Vector4 plane, BoundingBox boundingBox)
        {
            Plane = plane;
            Bounds = boundingBox;
        }
    }
}
