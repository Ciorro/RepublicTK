using System.Numerics;

namespace RepublicTK.Core.Serialization.Common
{
    public class Vector3Serializer : ISerializer<Vector3>
    {
        public Vector3 Read(BinaryReader reader, SerializationContext context)
        {
            return new Vector3(
                x: reader.ReadSingle(),
                y: reader.ReadSingle(),
                z: reader.ReadSingle()
            );
        }

        public void Write(BinaryWriter writer, SerializationContext context, Vector3 value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Z);
        }
    }
}
