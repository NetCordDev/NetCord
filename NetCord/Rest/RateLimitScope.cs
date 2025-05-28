namespace NetCord.Rest;

public enum RateLimitScope : sbyte
{
    User = 1 << 0,
    Global = 1 << 1,
    Shared = 1 << 2,
}

internal static class RateLimitScopeHelpers
{
    public static string GetString(RateLimitScope scope) => scope switch
    {
        RateLimitScope.User => nameof(RateLimitScope.User),
        RateLimitScope.Global => nameof(RateLimitScope.Global),
        RateLimitScope.Shared => nameof(RateLimitScope.Shared),
        _ => string.Empty,
    };
}
