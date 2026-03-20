namespace NetCord.Logging;

public interface IGatewayLogger
{
    public void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter);

    public bool IsEnabled(LogLevel logLevel);
}
