using RepublicTK.Serialization.Common.Models;
using System.Numerics;

namespace RepublicTK.Serialization.Models.Models
{
    public class Mesh : ModelNode
    {
        private const int MESH_HEADER_SIZE = 28;

        public List<short> Indices { get; set; } = [];
        public List<Vertex> Vertices { get; set; } = [];
        public List<Face> Faces { get; set; } = [];
        public List<Subset> Subsets { get; set; } = [];
        public VertexAttributes Attributes { get; set; }

        public Mesh()
        {
        }

        public Mesh(IEnumerable<Vertex> vertices, IEnumerable<short> indices)
        {
            Indices = new List<short>(indices);
            Vertices = new List<Vertex>(vertices);
        }

        public void CalculateFaces()
        {
            Faces = new List<Face>(Indices.Count / 3);

            for (int i = 0; i < Vertices.Count - 2; i += 3)
            {
                Vector3 p0 = Vertices[i + 0].Position;
                Vector3 p1 = Vertices[i + 1].Position;
                Vector3 p2 = Vertices[i + 2].Position;

                Vector3 u1 = p2 - p0;
                Vector3 u2 = p1 - p0;
                Vector3 n = Vector3.Normalize(Vector3.Cross(u1, u2));
                float d = -Vector3.Dot(n, p0);

                Vector3 min = Vector3.Min(p0, Vector3.Min(p1, p2));
                Vector3 max = Vector3.Max(p0, Vector3.Max(p1, p2));

                Vector4 plane = new Vector4(n, d);
                BoundingBox bounds = new BoundingBox(min, max);

                Faces.Add(new Face(plane, bounds));
            }
        }

        public void CalculateBounds()
        {
            if (Vertices.Count >= 2)
            {
                Vector3 min = Vertices.Select(x => x.Position).Aggregate((v1, v2) => Vector3.Min(v1, v2));
                Vector3 max = Vertices.Select(x => x.Position).Aggregate((v1, v2) => Vector3.Max(v1, v2));
                Bounds = new BoundingBox(min, max);
            }

            Bounds = BoundingBox.Zero;
        }

        internal override int GetSize()
        {
            return NODE_HEADER_SIZE + GetMeshSize();
        }

        internal int GetMeshSize()
        {
            int totalSize = MESH_HEADER_SIZE;

            int vertexSize = Attributes.GetVertexSize();
            totalSize += Vertices.Count * vertexSize;
            totalSize += Indices.Count * sizeof(short);
            totalSize += Faces.Count * Face.SIZE;
            totalSize += Subsets.Sum(x => x.GetSize());

            return totalSize;
        }
    }
}
