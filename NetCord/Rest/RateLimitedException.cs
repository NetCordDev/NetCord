using System.Runtime.Serialization;

namespace NetCord.Rest.RateLimits;

[Serializable]
public class RateLimitedException : Exception
{
    public long Reset { get; }
    public RateLimitScope Scope { get; }
    public long ResetAfter => Reset - Environment.TickCount64;

    public RateLimitedException(long reset, RateLimitScope scope) : base("Rate limit triggered.")
    {
        Reset = reset;
        Scope = scope;
    }

    protected RateLimitedException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        Reset = serializationInfo.GetInt64(nameof(Reset));
        Scope = (RateLimitScope)serializationInfo.GetSByte(nameof(Scope));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Reset), Reset);
        info.AddValue(nameof(Scope), (sbyte)Scope);
    }
}
