using RepublicTK.Core.Models;
using System.Numerics;
using System.Text;

namespace RepublicTK.Core.Extensions
{
    public static class BinaryWriterExtensions
    {
        public static void WriteCString(this BinaryWriter writer, string str, int maxLength)
        {
            writer.WriteCString(str, Encoding.ASCII, maxLength);
        }

        public static void WriteCString(this BinaryWriter writer, string str, Encoding encoding, int maxLength)
        {
            Span<byte> buffer = stackalloc byte[maxLength];

            if (encoding.TryGetBytes(str, buffer, out _))
            {
                writer.Write(buffer);
            }
        }

        public static void Write(this BinaryWriter writer, Vector2 vector2)
        {
            writer.Write(vector2.X);
            writer.Write(vector2.Y);
        }

        public static void Write(this BinaryWriter writer, Vector3 vector3)
        {
            writer.Write(vector3.X);
            writer.Write(vector3.Y);
            writer.Write(vector3.Z);
        }

        public static void Write(this BinaryWriter writer, Vector4 vector4)
        {
            writer.Write(vector4.X);
            writer.Write(vector4.Y);
            writer.Write(vector4.Z);
            writer.Write(vector4.W);
        }

        public static void Write(this BinaryWriter writer, Matrix4x4 matrix4x4)
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    writer.Write(matrix4x4[x, y]);
                }
            }
        }
    }
}
