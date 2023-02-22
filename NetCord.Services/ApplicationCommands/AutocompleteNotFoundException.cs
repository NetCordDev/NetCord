using System.Runtime.Serialization;

namespace NetCord.Services.ApplicationCommands;

[Serializable]
public class AutocompleteNotFoundException : Exception
{
    public AutocompleteNotFoundException() : base("Autocomplete not found.")
    {
    }

    protected AutocompleteNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
