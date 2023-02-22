using System.Runtime.Serialization;

namespace NetCord.Services.Commands;

[Serializable]
public class ParameterCountException : Exception
{
    public ParameterCountException(string? message) : base(message)
    {
    }

    protected ParameterCountException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
