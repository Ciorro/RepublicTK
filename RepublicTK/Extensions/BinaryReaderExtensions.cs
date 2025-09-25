using RepublicTK.Serialization.Common.Models;
using System.Numerics;
using System.Text;

namespace RepublicTK.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static string ReadCString(this BinaryReader reader, int maxLength)
        {
            return reader.ReadCString(Encoding.ASCII, maxLength);
        }

        public static string ReadCString(this BinaryReader reader, Encoding encoding, int maxLength)
        {
            byte[] buffer = reader.ReadBytes(maxLength);
            string str = encoding.GetString(buffer);

            int length = str.IndexOf('\0');
            if (length < 0)
            {
                length = str.Length;
            }

            return str.Substring(0, length);
        }

        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        public static Vector4 ReadVector4(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Vector4(x, y, z, w);
        }

        public static Matrix4x4 ReadMatrix4x4(this BinaryReader reader)
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

        public static BoundingBox ReadBoundingBox(this BinaryReader reader)
        {
            Vector3 min = reader.ReadVector3();
            Vector3 max = reader.ReadVector3();
            return new BoundingBox(min, max);
        }
    }
}
