namespace NetCord.Logging;

public class NullLogger : IGatewayLogger, IRestLogger, IVoiceLogger, IWebSocketLogger
{
    public static NullLogger Instance { get; } = new();

    private NullLogger()
    {
    }

    void IGatewayLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }

    void IRestLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }

    void IVoiceLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }

    void IWebSocketLogger.Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }
}
