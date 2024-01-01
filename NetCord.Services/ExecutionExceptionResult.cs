namespace NetCord.Services;

public class ExecutionExceptionResult : IExceptionResult
{
    public ExecutionExceptionResult(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }

    public string Message => Exception.Message;
}
