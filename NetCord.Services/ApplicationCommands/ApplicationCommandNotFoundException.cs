using System.Runtime.Serialization;

namespace NetCord.Services.ApplicationCommands;

[Serializable]
public class ApplicationCommandNotFoundException : Exception
{
    public ApplicationCommandNotFoundException() : base("Command not found.")
    {
    }

    protected ApplicationCommandNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
