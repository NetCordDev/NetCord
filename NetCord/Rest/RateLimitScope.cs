namespace NetCord.Rest;

public enum RateLimitScope : sbyte
{
    User = 1 << 0,
    Global = 1 << 1,
    Shared = 1 << 2,
}
