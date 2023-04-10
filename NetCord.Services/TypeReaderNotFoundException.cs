using System.Runtime.Serialization;

namespace NetCord.Services;

[Serializable]
public class TypeReaderNotFoundException : Exception
{
    public TypeReaderNotFoundException(Type type) : base($"Type name: '{type}'.")
    {
    }

    public TypeReaderNotFoundException(Type type, Type type2) : base($"Type name: '{type}' or '{type2}'.")
    {
    }

    protected TypeReaderNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
