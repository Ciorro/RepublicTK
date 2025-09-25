using System.Numerics;

namespace RepublicTK.Serialization.Models.Models
{
    public class Vertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector3 Tangent { get; set; }
        public Vector3 Bitangent { get; set; }
        public Vector2 UV { get; set; }
        public short[] BoneWeights { get; }
        public byte[] BoneIndices { get; }

        public Vertex(Vector3 position)
        {
            Position = position;

            BoneWeights = new short[4];
            BoneIndices = new byte[4];
        }
    }
}
