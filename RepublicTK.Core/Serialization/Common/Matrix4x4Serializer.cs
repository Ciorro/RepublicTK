using System.Numerics;

namespace RepublicTK.Core.Serialization.Common
{
    public class Matrix4x4Serializer : ISerializer<Matrix4x4>
    {
        public Matrix4x4 Read(BinaryReader reader, SerializationContext context)
        {
            var matrix = new Matrix4x4();

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    matrix[x, y] = reader.ReadSingle();
                }
            }

            return matrix;
        }

        public void Write(BinaryWriter writer, SerializationContext context, Matrix4x4 value)
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    writer.Write(value[x, y]);
                }
            }
        }
    }
}
