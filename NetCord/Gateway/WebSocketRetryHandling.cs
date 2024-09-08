namespace NetCord.Gateway;

[Flags]
public enum WebSocketRetryHandling : byte
{
    NoRetry = 0,
    RetryRateLimit = 1 << 0,
    RetryReconnect = 1 << 1,
    Retry = RetryRateLimit | RetryReconnect,
}
