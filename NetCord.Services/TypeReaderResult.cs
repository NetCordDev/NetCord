namespace NetCord.Services;

public abstract class TypeReaderResult : IExecutionResult
{
    public static TypeReaderResult Success(object value) => new TypeReaderSuccessResult(value);

    public static TypeReaderResult Fail(string message) => new TypeReaderFailResult(message);

    public static TypeReaderResult ParseFail(string parameterName) => Fail($"Failed to parse '{parameterName}'.");
}

public class TypeReaderSuccessResult(object value) : TypeReaderResult
{
    public object Value { get; } = value;
}

public class TypeReaderFailResult(string message) : TypeReaderResult, IFailResult
{
    public string Message { get; } = message;
}

public class TypeReaderExceptionResult(Exception exception) : TypeReaderFailResult(exception.Message)
{
    public Exception Exception { get; } = exception;
}
