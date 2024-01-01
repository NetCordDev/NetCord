namespace NetCord.Services;

public abstract class PreconditionResult : IExecutionResult
{
    public static PreconditionSuccessResult Success { get; } = new();

    public static PreconditionFailResult Fail(string message) => new(message);
}

public class PreconditionSuccessResult : PreconditionResult
{
}

public class PreconditionFailResult : PreconditionResult, IFailResult
{
    public PreconditionFailResult(string message)
    {
        Message = message;
    }

    public string Message { get; }
}

public class PreconditionExceptionResult : PreconditionFailResult, IExceptionResult
{
    public PreconditionExceptionResult(Exception exception) : base(exception.Message)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}
