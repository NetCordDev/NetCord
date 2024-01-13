namespace NetCord.Gateway;

public class LogMessage : ISpanFormattable
{
    private const string ErrorSeverity = "Error";
    private const string InfoSeverity = "Info";

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

    public static LogMessage Error(Exception exception) => new(exception);

    public static LogMessage Info(string message, string? description = null) => new(message, description);

    public string Message { get; }

    public LogSeverity Severity { get; }

    public string? Description { get; }

    public Exception? Exception { get; }

    public override string ToString()
    {
        if (Severity is LogSeverity.Error)
            return $"{DateTime.Now:T} {(Severity is LogSeverity.Error ? ErrorSeverity : InfoSeverity)}\t{Exception}";
        else
        {
            var description = Description;
            if (description is null)
                return $"{DateTime.Now:T} {(Severity is LogSeverity.Error ? ErrorSeverity : InfoSeverity)}\t{Message}";
            else
                return $"{DateTime.Now:T} {(Severity is LogSeverity.Error ? ErrorSeverity : InfoSeverity)}\t{Message}: {description}";
        }
    }

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        int written;

        if (Severity is LogSeverity.Error)
        {
            var severity = ErrorSeverity;
            var exceptionString = Exception!.ToString();

            int requiredLength = exceptionString.Length + severity.Length + 2;
            if (destination.Length < requiredLength || !DateTime.Now.TryFormat(destination[..^requiredLength], out var length, "T"))
            {
                charsWritten = 0;
                return false;
            }

            written = length;

            destination[written++] = ' ';
            severity.CopyTo(destination[written..]);
            written += severity.Length;
            destination[written++] = '\t';
            exceptionString.CopyTo(destination[written..]);
            written += exceptionString.Length;
        }
        else
        {
            var message = Message;
            var severity = InfoSeverity;
            var description = Description;

            int requiredLength = description is null ? message.Length + severity.Length + 2 : message.Length + description.Length + severity.Length + 4;
            if (destination.Length < requiredLength || !DateTime.Now.TryFormat(destination[..^requiredLength], out var length, "T"))
            {
                charsWritten = 0;
                return false;
            }

            written = length;

            destination[written++] = ' ';
            severity.CopyTo(destination[written..]);
            written += severity.Length;
            destination[written++] = '\t';
            message.CopyTo(destination[written..]);
            written += message.Length;

            if (description is not null)
            {
                ": ".CopyTo(destination[written..]);
                written += 2;
                description.CopyTo(destination[written..]);
                written += description.Length;
            }
        }

        charsWritten = written;
        return true;
    }
}
