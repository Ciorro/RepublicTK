using RepublicTK.Core.Serialization;
using RepublicTK.Trees.Serialization;

namespace RepublicTK.Trees.Extensions
{
    public static class SerializationContextExtensions
    {
        public static SerializationContext AddTreeSerializers(this SerializationContext context)
        {
            context.Register(new TreesSerializer());
            context.Register(new TreeGroupSerializer());
            return context;
        }
    }
}
