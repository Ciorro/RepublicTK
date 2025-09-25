using System.Numerics;

namespace RepublicTK.Serialization.Models.Models
{
    public class Helper : ModelNode
    {
        public Matrix4x4 TransformMatrix { get; set; }

        internal override int GetSize()
        {
            return NODE_HEADER_SIZE + 64;
        }
    }
}
