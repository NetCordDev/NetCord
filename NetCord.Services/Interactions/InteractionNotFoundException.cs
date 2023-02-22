using System.Runtime.Serialization;

namespace NetCord.Services.Interactions;

[Serializable]
public class InteractionNotFoundException : Exception
{
    public InteractionNotFoundException() : base("Interaction not found.")
    {
    }

    protected InteractionNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
