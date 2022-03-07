namespace NetCord;

public class LogMessage
{
    private LogMessage(string message)
    {
        Message = message;
        Severity = LogSeverity.Info;
    }

    private LogMessage(Exception exception)
    {
        Message = exception.Message;
        Exception = exception;
        Severity = LogSeverity.Error;
    }

    internal static LogMessage Error(Exception exception)
    {
        return new(exception);
    }

    internal static LogMessage Info(string message)
    {
        return new(message);
    }

    public string Message { get; }

    public LogSeverity Severity { get; }

    public Exception? Exception { get; }

    public override string ToString()
    {
        if (Severity == LogSeverity.Error)
            return $"{DateTime.Now:T} {Severity}\t{Exception}";
        else
            return $"{DateTime.Now:T} {Severity}\t{Message}";
    }
}