namespace RepublicTK.Serialization.Extensions
{
    public static class SerializerExtensions
    {
        public static T Read<T>(this ISerializer<T> serializer, Stream input)
        {
            using (var reader = new BinaryReader(input))
            {
                return serializer.Read(reader);
            }
        }

        public static T Read<T>(this ISerializer<T> serializer, string fileName)
        {
            using (var fs = File.OpenRead(fileName))
            {
                return serializer.Read(fs);
            }
        }

        public static void Write<T>(this ISerializer<T> serializer, Stream output, T value)
        {
            using (var writer = new BinaryWriter(output))
            {
                serializer.Write(writer, value);
            }
        }

        public static void Write<T>(this ISerializer<T> serializer, string fileName, T value)
        {
            using (var fs = File.OpenWrite(fileName))
            {
                serializer.Write(fs, value);
            }
        }
    }
}
