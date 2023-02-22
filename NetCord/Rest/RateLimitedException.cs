using System.Runtime.Serialization;

namespace NetCord.Rest.RateLimits;

[Serializable]
public class RateLimitedException : Exception
{
    public bool Global { get; }
    public long Reset { get; }

    public RateLimitedException(long reset, bool global) : base("Rate limit triggered.")
    {
        Reset = reset;
        Global = global;
    }

    protected RateLimitedException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        Global = serializationInfo.GetBoolean(nameof(Global));
        Reset = serializationInfo.GetInt64(nameof(Reset));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Global), Global);
        info.AddValue(nameof(Reset), Reset);
    }
}
