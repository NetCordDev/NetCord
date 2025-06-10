namespace NetCord.Logging;

public class NullLogger : IGatewayLogger, IRestLogger, IVoiceLogger, IWebSocketLogger
{
    public static NullLogger Instance { get; } = new();

    private NullLogger()
    {
    }

    bool IGatewayLogger.IsEnabled(LogLevel logLevel) => false;

    bool IRestLogger.IsEnabled(LogLevel logLevel) => false;

    bool IVoiceLogger.IsEnabled(LogLevel logLevel) => false;

    bool IWebSocketLogger.IsEnabled(LogLevel logLevel) => false;

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
