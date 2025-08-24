using RepublicTK.Core.Serialization;
using RepublicTK.Core.Serialization.Common;

namespace RepublicTK.Core.Extensions
{
    public static class SerializationContextExtensions
    {
        public static T Read<T>(this SerializationContext context, BinaryReader reader)
        {
            return context.GetSerializer<T>().Read(reader, context);
        }

        public static void Write<T>(this SerializationContext context, BinaryWriter writer, T value)
        {
            context.GetSerializer<T>().Write(writer, context, value);
        }

        public static SerializationContext AddCoreSerializers(this SerializationContext context)
        {
            context.Register(new Vector3Serializer());
            context.Register(new RegionSerializer());
            context.Register(new Matrix4x4Serializer());
            return context;
        }
    }
}
