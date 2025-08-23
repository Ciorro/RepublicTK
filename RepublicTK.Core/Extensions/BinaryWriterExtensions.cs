using System.Text;

namespace RepublicTK.Core.Extensions
{
    public static class BinaryWriterExtensions
    {
        public static void WriteCString(this BinaryWriter writer, string str, Encoding encoding, int maxLength)
        {
            Span<byte> buffer = stackalloc byte[maxLength];

            if (encoding.TryGetBytes(str, buffer, out _))
            {
                writer.Write(buffer);
            }
        }
    }
}
