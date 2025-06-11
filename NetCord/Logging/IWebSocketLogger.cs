namespace NetCord.Logging;

internal interface IWebSocketLogger
{
    public bool IsEnabled(LogLevel logLevel);

    public void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter) where TState : allows ref struct;
}
