using System.Numerics;

namespace RepublicTK.Core.Models
{
    public struct BoundingBox
    {
        public Vector3 Min;
        public Vector3 Max;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public Vector3 Size
        {
            get => Max - Min;
        }

        public static readonly BoundingBox Zero = new BoundingBox(Vector3.Zero, Vector3.Zero);
    }
}
