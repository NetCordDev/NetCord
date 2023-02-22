using System.Runtime.Serialization;

namespace NetCord.Gateway.Voice;

[Serializable]
public class OpusException : Exception
{
    public OpusError OpusError { get; }

    public OpusException(OpusError error) : base(error.ToString())
    {
        OpusError = error;
    }

    protected OpusException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        OpusError = (OpusError)serializationInfo.GetInt32(nameof(OpusError));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(OpusError), (int)OpusError);
    }
}
