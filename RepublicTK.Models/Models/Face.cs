using RepublicTK.Core.Models;
using System.Numerics;

namespace RepublicTK.Models.Models
{
    public class Face
    {
        internal const int FACE_SIZE = 40;

        public Vector4 Plane { get; set; }
        public BoundingBox Bounds { get; set; }

        public Face(Vector4 plane)
        {
            Plane = plane;
        }

        public Face(Vector4 plane, BoundingBox boundingBox)
            : this(plane)
        {
            Bounds = boundingBox;
        }
    }
}
