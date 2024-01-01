namespace NetCord.Services;

public interface IExecutionResult
{
}

public interface IFailResult : IExecutionResult
{
    public string Message { get; }
}

public interface IExceptionResult : IFailResult
{
    public Exception Exception { get; }
}
