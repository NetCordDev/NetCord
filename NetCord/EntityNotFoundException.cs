using System.Runtime.Serialization;

namespace NetCord;

[Serializable]
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException() : base()
    {
    }

    public EntityNotFoundException(string? message) : base(message)
    {
    }

    protected EntityNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
