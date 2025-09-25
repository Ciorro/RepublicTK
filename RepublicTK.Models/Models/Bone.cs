using System.Numerics;

namespace RepublicTK.Models.Models
{
    public class Bone : ModelNode
    {
        public Matrix4x4 TransformMatrix { get; set; }

        internal override int GetSize()
        {
            return NODE_HEADER_SIZE + 64;
        }
    }
}
