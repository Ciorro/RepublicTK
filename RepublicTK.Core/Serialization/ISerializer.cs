namespace RepublicTK.Core.Serialization
{
    public interface ISerializer<T>
    {
        T Read(BinaryReader reader);
        void Write(BinaryWriter writer, T value);
    }
}
