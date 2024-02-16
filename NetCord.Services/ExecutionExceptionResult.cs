namespace NetCord.Services;

public class ExecutionExceptionResult(Exception exception) : IExceptionResult
{
    public Exception Exception { get; } = exception;

    public string Message => Exception.Message;
}
