namespace NetCord.Services;

public abstract class TypeReaderResult : IExecutionResult
{
    public static TypeReaderResult Success(object value) => new TypeReaderSuccessResult(value);

    public static TypeReaderResult Fail(string message) => new TypeReaderFailResult(message);

    public static TypeReaderResult ParseFail(string parameterName) => Fail($"Failed to parse '{parameterName}'.");
}

public class TypeReaderSuccessResult : TypeReaderResult
{
    public TypeReaderSuccessResult(object value)
    {
        Value = value;
    }

    public object Value { get; }
}

public class TypeReaderFailResult : TypeReaderResult, IFailResult
{
    public TypeReaderFailResult(string message)
    {
        Message = message;
    }

    public string Message { get; }
}

public class TypeReaderExceptionResult : TypeReaderFailResult
{
    public TypeReaderExceptionResult(Exception exception) : base(exception.Message)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}
