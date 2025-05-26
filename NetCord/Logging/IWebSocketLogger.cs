namespace NetCord.Logging;

internal interface IWebSocketLogger
{
    public void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter);
}
