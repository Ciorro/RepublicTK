using RepublicTK.Serialization.Extensions;
using RepublicTK.Serialization.Models.Models;

namespace RepublicTK.Serialization.Models.Serialization
{
    public sealed partial class ModelSerializer : ISerializer<Model>
    {
        public Model Read(BinaryReader reader)
        {
            var model = new Model();

            string header = reader.ReadCString(8);
            if (!IsValidHeader(header))
            {
                throw new InvalidDataException($"Invalid file header \"{header}\". Expected \"B3DMH\" or \"fromObj\"");
            }

            int materialCount = reader.ReadInt32();
            int modelNodeCount = reader.ReadInt32();
            int size = reader.ReadInt32();

            // Read materials
            for (int i = 0; i < materialCount; i++)
            {
                model.Materials.Add(reader.ReadCString(64));
            }

            // Read nodes
            for (int i = 0; i < modelNodeCount; i++)
            {
                int nodeType = reader.ReadInt32();

                switch (nodeType)
                {
                    case 0:
                        model.Nodes.Add(ReadMeshNode(reader));
                        break;
                    case 1:
                        model.Nodes.Add(ReadBoneNode(reader));
                        break;
                    case 2:
                        model.Nodes.Add(ReadHelperNode(reader));
                        break;
                    default:
                        throw new InvalidDataException($"Invalid node type ({nodeType}).");
                }
            }

            return model;
        }

        private Mesh ReadMeshNode(BinaryReader reader)
        {
            var node = ReadModelNode<Mesh>(reader);

            int lodCount = reader.ReadInt32();
            int lodSize = reader.ReadInt32();

            int vertexCount = reader.ReadInt32();
            int indexCount = reader.ReadInt32();
            int subsetCount = reader.ReadInt32();

            int morphTargetsCount = reader.ReadInt32();
            MeshAttributes attr = reader.ReadInt32();
            int morphMask = reader.ReadInt32();

            node.Indices.Capacity = indexCount;
            node.Vertices.Capacity = vertexCount;
            node.Attributes = attr;

            // Read indices
            for (int i = 0; i < indexCount; i++)
            {
                node.Indices.Add(reader.ReadInt16());
            }

            // Read vertex positions
            for (int i = 0; i < vertexCount; i++)
            {
                node.Vertices.Add(new Vertex(reader.ReadVector3()));
            }

            if (attr.HasNormals)
            {
                // Read normals
                for (int i = 0; i < vertexCount; i++)
                {
                    node.Vertices[i].Normal = reader.ReadVector3();
                }
            }

            if (attr.HasTangents)
            {
                // Read tangents
                for (int i = 0; i < vertexCount; i++)
                {
                    node.Vertices[i].Tangent = reader.ReadVector3();
                }
            }

            if (attr.HasBitangents)
            {
                // Read bitangents
                for (int i = 0; i < vertexCount; i++)
                {
                    node.Vertices[i].Bitangent = reader.ReadVector3();
                }
            }

            if (attr.HasUVs)
            {
                // Read uvs
                for (int i = 0; i < vertexCount; i++)
                {
                    node.Vertices[i].UV = reader.ReadVector2();
                }
            }

            if (attr.HasBones)
            {
                // Read bone weights
                for (int i = 0; i < vertexCount; i++)
                {
                    int boneCount = node.Vertices[i].BoneWeights.Length;
                    for (int j = 0; j < boneCount; j++)
                    {
                        node.Vertices[i].BoneWeights[j] = reader.ReadInt16();
                    }
                }

                // Read bone indices
                for (int i = 0; i < vertexCount; i++)
                {
                    int boneCount = node.Vertices[i].BoneIndices.Length;
                    for (int j = 0; j < boneCount; j++)
                    {
                        node.Vertices[i].BoneIndices[j] = reader.ReadByte();
                    }
                }
            }

            if (attr.HasFaces)
            {
                var faces = new List<Face>(indexCount / 3);

                // Read faces
                for (int i = 0; i < faces.Capacity; i++)
                {
                    faces.Add(new Face(reader.ReadVector4(), default));
                }

                // Read faces bounds
                for (int i = 0; i < faces.Capacity; i++)
                {
                    faces[i] = faces[i] with
                    {
                        Bounds = reader.ReadBoundingBox()
                    };
                }

                node.Faces = faces;
            }

            // Read subsets
            for (int i = 0; i < subsetCount; i++)
            {
                node.Subsets.Add(ReadSubset(reader));
            }

            return node;
        }

        private Bone ReadBoneNode(BinaryReader reader)
        {
            var node = ReadModelNode<Bone>(reader);
            node.TransformMatrix = reader.ReadMatrix4x4();
            return node;
        }

        private Helper ReadHelperNode(BinaryReader reader)
        {
            var node = ReadModelNode<Helper>(reader);
            node.TransformMatrix = reader.ReadMatrix4x4();
            return node;
        }

        private T ReadModelNode<T>(BinaryReader reader)
            where T : ModelNode, new()
        {
            var modelNode = new T();
            int size = reader.ReadInt32();

            modelNode.Name = reader.ReadCString(64);
            modelNode.ParentId = reader.ReadInt16();
            modelNode.ChildrenCount = reader.ReadInt16();
            modelNode.WorldMatrix = reader.ReadMatrix4x4();
            modelNode.LocalMatrix = reader.ReadMatrix4x4();
            modelNode.Bounds = reader.ReadBoundingBox();

            return modelNode;
        }

        private Subset ReadSubset(BinaryReader reader)
        {
            var subset = new Subset();

            subset.StartIndex = reader.ReadInt32();
            subset.IndexCount = reader.ReadInt32();
            subset.MaterialId = reader.ReadInt16();

            short boneCount = reader.ReadInt16();
            for (int i = 0; i < boneCount; i++)
            {
                subset.BoneIds.Add(reader.ReadInt16());
            }

            return subset;
        }

        private bool IsValidHeader(string header)
        {
            return header == "B3DMH" || header == "fromObj";
        }
    }
}
