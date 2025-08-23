using System.Text;

namespace RepublicTK.Core.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static string ReadCString(this BinaryReader reader, Encoding encoding, int maxLength)
        {
            byte[] buffer = reader.ReadBytes(maxLength);
            string str = encoding.GetString(buffer);
            str = str.Substring(0, str.IndexOf('\0'));

            return str;
        }
    }
}
