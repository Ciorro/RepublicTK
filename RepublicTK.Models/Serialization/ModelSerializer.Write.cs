using RepublicTK.Core.Extensions;
using RepublicTK.Core.Serialization;
using RepublicTK.Models.Models;

namespace RepublicTK.Models.Serialization
{
    public sealed partial class ModelSerializer : ISerializer<Model>
    {
        public void Write(BinaryWriter writer, Model value)
        {
            writer.WriteCString("B3DMH\010", 8);
            writer.Write(value.Materials.Count);
            writer.Write(value.Nodes.Count);
            writer.Write(CalculateSize(value));

            // Write materials
            foreach (var material in value.Materials)
            {
                writer.WriteCString(material, 64);
            }

            foreach (var node in value.Nodes)
            {
                switch (node)
                {
                    case Mesh mesh:
                        writer.Write(0);
                        WriteMeshNode(writer, mesh);
                        break;
                    case Bone bone:
                        writer.Write(1);
                        WriteBoneNode(writer, bone);
                        break;
                    case Helper helper:
                        writer.Write(2);
                        WriteHelperNode(writer, helper);
                        break;
                }
            }
        }

        private void WriteMeshNode(BinaryWriter writer, Mesh node)
        {
            WriteModelNode(writer, node);

            writer.Write(1);
            writer.Write(node.GetMeshSize());
            writer.Write(node.Vertices.Count);
            writer.Write(node.Indices.Count);
            writer.Write(node.Subsets.Count);
            writer.Write(0);
            writer.Write(node.Attributes);
            writer.Write(0);

            // Write indices
            foreach (var index in node.Indices)
            {
                writer.Write(index);
            }

            // Write vertex positions
            foreach (var vertex in node.Vertices)
            {
                writer.Write(vertex.Position);
            }

            if (node.Attributes.HasNormals)
            {
                // Write normals
                foreach (var vertex in node.Vertices)
                {
                    writer.Write(vertex.Normal);
                }
            }

            if (node.Attributes.HasTangents)
            {
                // Write tangents
                foreach (var vertex in node.Vertices)
                {
                    writer.Write(vertex.Tangent);
                }
            }

            if (node.Attributes.HasBitangents)
            {
                // Write bitangents
                foreach (var vertex in node.Vertices)
                {
                    writer.Write(vertex.Bitangent);
                }
            }

            if (node.Attributes.HasUVs)
            {
                // Write uvs
                foreach (var vertex in node.Vertices)
                {
                    writer.Write(vertex.UV);
                }
            }

            if (node.Attributes.HasBones)
            {
                // Write bone weights
                foreach (var vertex in node.Vertices)
                {
                    int boneCount = vertex.BoneWeights.Length;
                    for (int i = 0; i < boneCount; i++)
                    {
                        writer.Write(vertex.BoneWeights[i]);
                    }
                }

                // Write bone indices
                foreach (var vertex in node.Vertices)
                {
                    int boneCount = vertex.BoneIndices.Length;
                    for (int i = 0; i < boneCount; i++)
                    {
                        writer.Write(vertex.BoneIndices[i]);
                    }
                }
            }

            if (node.Attributes.HasFaces)
            {
                // Write faces
                foreach (var face in node.Faces)
                {
                    writer.Write(face.Plane);
                }

                // Write faces bounds
                foreach (var face in node.Faces)
                {
                    writer.Write(face.Bounds);
                }
            }

            // Write subsets
            foreach (var subset in node.Subsets)
            {
                WriteSubset(writer, subset);
            }
        }

        private void WriteBoneNode(BinaryWriter writer, Bone node)
        {
            WriteModelNode(writer, node);
            writer.Write(node.TransformMatrix);
        }

        private void WriteHelperNode(BinaryWriter writer, Helper node)
        {
            WriteModelNode(writer, node);
            writer.Write(node.TransformMatrix);
        }

        private void WriteModelNode<T>(BinaryWriter writer, T node)
            where T : ModelNode
        {
            writer.Write(node.GetSize());
            writer.WriteCString(node.Name, 64);
            writer.Write(node.ParentId);
            writer.Write(node.ChildrenCount);
            writer.Write(node.WorldMatrix);
            writer.Write(node.LocalMatrix);
            writer.Write(node.Bounds);
        }

        private void WriteSubset(BinaryWriter writer, Subset subset)
        {
            writer.Write(subset.StartIndex);
            writer.Write(subset.IndexCount);
            writer.Write(subset.MaterialId);
            writer.Write((short)subset.BoneIds.Count);

            foreach (var boneId in subset.BoneIds)
            {
                writer.Write(boneId);
            }
        }

        private int CalculateSize(Model value)
        {
            int totalSize = 20;
            totalSize += value.Materials.Count * 64;
            totalSize += value.Nodes.Sum(x => x.GetSize());
            return totalSize;
        }
    }
}
