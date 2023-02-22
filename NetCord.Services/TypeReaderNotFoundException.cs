using System.Runtime.Serialization;

namespace NetCord.Services;

[Serializable]
public class TypeReaderNotFoundException : Exception
{
    public TypeReaderNotFoundException(string typeName) : base(typeName)
    {
    }

    protected TypeReaderNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
