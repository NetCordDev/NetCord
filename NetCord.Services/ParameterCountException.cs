using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetCord.Services;

[Serializable]
public class ParameterCountException : Exception
{
    public ParameterCountExceptionType Type { get; }

    public ParameterCountException(ParameterCountExceptionType type) : base(type switch
    {
        ParameterCountExceptionType.TooFew => "Too few parameters.",
        ParameterCountExceptionType.TooMany => "Too many parameters.",
        _ => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(ParameterCountExceptionType)),
    })
    {
        Type = type;
    }

    protected ParameterCountException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        Type = (ParameterCountExceptionType)serializationInfo.GetInt32(nameof(Type));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Type), (int)Type);
    }
}
