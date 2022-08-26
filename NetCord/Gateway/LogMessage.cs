namespace NetCord.Gateway;

public class LogMessage
{
    private LogMessage(string message)
    {
        Message = message;
        Severity = LogSeverity.Info;
    }

    private LogMessage(string message, string? description)
    {
        Message = message;
        Severity = LogSeverity.Info;
        Description = description;
    }

    private LogMessage(Exception exception)
    {
        Message = exception.Message;
        Exception = exception;
        Severity = LogSeverity.Error;
    }

    internal static LogMessage Error(Exception exception) => new(exception);

    internal static LogMessage Info(string message) => new(message);

    internal static LogMessage Info(string message, string? description) => new(message, description);

    public string Message { get; }

    public LogSeverity Severity { get; }

    public string? Description { get; }

    public Exception? Exception { get; }

    public override string ToString()
    {
        if (Severity == LogSeverity.Error)
            return $"{DateTime.Now:T} {Severity}\t{Exception}";
        else if (Description == null)
            return $"{DateTime.Now:T} {Severity}\t{Message}";
        else
            return $"{DateTime.Now:T} {Severity}\t{Message}: {Description}";
    }
}
