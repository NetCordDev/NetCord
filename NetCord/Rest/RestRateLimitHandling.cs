namespace NetCord.Rest;

[Flags]
public enum RestRateLimitHandling : sbyte
{
    NoRetry = 0,
    RetryUser = 1 << 0,
    RetryGlobal = 1 << 1,
    RetryShared = 1 << 2,
    Retry = RetryUser | RetryGlobal | RetryShared,
}
