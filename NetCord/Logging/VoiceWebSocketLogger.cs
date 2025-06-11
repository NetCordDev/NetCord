namespace NetCord.Logging;

internal class VoiceWebSocketLogger(IVoiceLogger logger) : IWebSocketLogger
{
    public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter) where TState : allows ref struct
    {
        logger.Log(logLevel, state, exception, formatter);
    }
}
