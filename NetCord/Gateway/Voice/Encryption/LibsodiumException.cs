using System.Runtime.Serialization;

namespace NetCord.Gateway.Voice.Encryption;

[Serializable]
public class LibsodiumException : Exception
{
    public LibsodiumException() : base("Libsodium returned an error.")
    {
    }

    protected LibsodiumException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}
