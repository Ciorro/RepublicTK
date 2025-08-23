namespace RepublicTK.Core.Serialization
{
    public interface ISerializer<T>
    {
        T Read(BinaryReader reader, SerializationContext context);
        void Write(BinaryWriter writer, SerializationContext context, T value);
    }
}
