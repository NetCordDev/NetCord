namespace NetCord.Logging;

internal class GatewayWebSocketLogger(IGatewayLogger logger) : IWebSocketLogger
{
    public void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        logger.Log(logLevel, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logger.IsEnabled(logLevel);
    }
}
