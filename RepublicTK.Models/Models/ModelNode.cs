using RepublicTK.Core.Models;
using System.Numerics;

namespace RepublicTK.Models.Models
{
    public abstract class ModelNode
    {
        protected const int NODE_HEADER_SIZE = 228;

        public string Name { get; set; } = "";
        public short ParentId { get; set; } = -1;
        public short ChildrenCount { get; set; }
        public Matrix4x4 WorldMatrix { get; set; }
        public Matrix4x4 LocalMatrix { get; set; }
        public BoundingBox Bounds { get; set; }

        public ModelNode()
        {
            WorldMatrix = Matrix4x4.Identity;
            LocalMatrix = Matrix4x4.Identity;
        }

        internal abstract int GetSize();
    }
}
