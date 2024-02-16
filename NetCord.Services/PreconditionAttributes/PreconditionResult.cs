namespace NetCord.Services;

public abstract class PreconditionResult : IExecutionResult
{
    public static PreconditionSuccessResult Success { get; } = new();

    public static PreconditionFailResult Fail(string message) => new(message);
}

public class PreconditionSuccessResult : PreconditionResult
{
}

public class PreconditionFailResult(string message) : PreconditionResult, IFailResult
{
    public string Message { get; } = message;
}

public class PreconditionExceptionResult(Exception exception) : PreconditionFailResult(exception.Message), IExceptionResult
{
    public Exception Exception { get; } = exception;
}
