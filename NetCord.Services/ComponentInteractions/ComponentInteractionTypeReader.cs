namespace NetCord.Services.ComponentInteractions;

public abstract class ComponentInteractionTypeReader<TContext> : IInteractionTypeReader where TContext : IComponentInteractionContext
{
    public abstract ValueTask<ComponentInteractionTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
}

internal interface IInteractionTypeReader
{
}

public abstract class ComponentInteractionTypeReaderResult : IExecutionResult
{
    public static ComponentInteractionTypeReaderResult Success(object? value) => new ComponentInteractionTypeReaderSuccessResult(value);

    public static ComponentInteractionTypeReaderResult Fail(string message) => new ComponentInteractionTypeReaderFailResult(message);

    public static ComponentInteractionTypeReaderResult ParseFail(string parameterName) => Fail($"Failed to parse '{parameterName}'.");
}

public class ComponentInteractionTypeReaderSuccessResult(object? value) : ComponentInteractionTypeReaderResult
{
    public object? Value { get; } = value;
}

public class ComponentInteractionTypeReaderFailResult(string message) : ComponentInteractionTypeReaderResult, IFailResult
{
    public string Message { get; } = message;
}

public class ComponentInteractionTypeReaderExceptionResult(Exception exception) : ComponentInteractionTypeReaderFailResult(exception.Message)
{
    public Exception Exception { get; } = exception;
}
