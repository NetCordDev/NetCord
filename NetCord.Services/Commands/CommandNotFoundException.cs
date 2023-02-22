using System.Runtime.Serialization;

namespace NetCord.Services.Commands;

[Serializable]
public class CommandNotFoundException : Exception
{
    public CommandNotFoundException() : base("Command not found.")
    {
    }

    protected CommandNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
