namespace NetCord.Logging;

public interface IRestLogger
{
    public void Log<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter);

    public bool IsEnabled(LogLevel logLevel);
}
