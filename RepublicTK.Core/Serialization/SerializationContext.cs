namespace RepublicTK.Core.Serialization
{
    public class SerializationContext
    {
        public Dictionary<Type, object> Serializers { get; } = new Dictionary<Type, object>();

        public void Register(object serializer)
        {
            var type = serializer.GetType();

            if (type.IsAbstract)
            {
                throw new ArgumentException("Serializer cannot be an abstract class.");
            }

            var serializerInterface = type.GetInterfaces()
                .SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISerializer<>));

            if (serializerInterface is null)
            {
                throw new ArgumentException($"Serializer must implement ISerializer<T> interface.");
            }

            var serializedType = serializerInterface.GetGenericArguments().Single();
            Serializers[serializedType] = serializer;
        }

        public void Register<T>(ISerializer<T> serializer)
        {
            Serializers[typeof(T)] = serializer;
        }

        public ISerializer<T> GetSerializer<T>()
        {
            if (Serializers.TryGetValue(typeof(T), out var serializer))
            {
                return (ISerializer<T>)serializer;
            }

            throw new InvalidOperationException($"No serializer registered for type {typeof(T)}");
        }
    }
}
